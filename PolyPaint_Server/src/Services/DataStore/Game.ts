import { DocumentsToArray } from "./MongoDB/utils"
import * as uuid from "uuid/v4"
import * as ConversationActions from "./Conversations"
import * as UserActions from "./User"
import { IGame, Game, GAME_STATE, GAME_UPDATE } from "../../Models/Game";
import { ICreateGameRequest, IUpdateGameRequest, IAddPlayerRequest, IDeleteGame, IAddVirtualPlayerRequest } from "../../Interfaces/Game";
import { mongo } from "./MongoDB/index"
import { Team, ITeam } from "../../Models/Team";
import { Conversation } from "../../Models/Conversation";
import { getCurrentTimestamp } from "../Time/GetTime";
import { Player, IPlayerUser, PlayerUser } from "../../Models/Player";
import { IUser, User } from "../../Models/User";
import { EmitInRoom } from "../../Socket";
import GameController from "../../Socket/Controllers/game"
import ConversationController from "../../Socket/Controllers/conversation"
import { Context } from "koa";
import { IAction, GAME_ACTIONS } from "../../Models/Action";
import GameFlow from "../GameFlow";
import { Cursor } from "mongodb";
import { PERSONALITY, VirtualPlayer } from "../../Models/VirtualPlayer";
import * as VirtualPlayerActions from "../../Services/DataStore/VirtualPlayer"
import axios from "axios"

const COLLECTION = "Game"

export async function getAll(): Promise<Array<IGame>> {
    const data = await mongo(COLLECTION).find().toArray()

    return DocumentsToArray<IGame>(data, toGame)
}

export async function getActive(): Promise<Array<IGame>> {
    const data = await mongo(COLLECTION).find({ state: GAME_STATE.LOBBY }).sort({ timestamp: -1 }).toArray()

    return DocumentsToArray<IGame>(data, toGame)
}

export async function get(_id: string): Promise<IGame> {

    const data = await mongo(COLLECTION).findOne({ _id })

    return data ? toGame(data) : null
}

export async function getByConversation(conversation_id: string): Promise<IGame> {

    const data = await mongo(COLLECTION).findOne({ conversation_id })

    return data ? toGame(data) : null
}


export async function create(game: ICreateGameRequest): Promise<IGame> {

    const _id = uuid()

    const { data } = await axios.get("http://names.drycodes.com/1?nameOptions=planets,games&separator=space")

    console.log("NAMES", data)
    const name = data[0]
    
    let newConversation = new Conversation(uuid(), name, getCurrentTimestamp(), getCurrentTimestamp(), [game.uid], []) 
    let conversation = await ConversationActions.create(newConversation)

    let user: IUser = await UserActions.getBasic(game.uid)    
    let playerUser: IPlayerUser = new PlayerUser(user.uid, user.username, user.profileImgUrl)

    let team1 = new Team(uuid(), new Player(uuid(), game.uid, playerUser), null, 0)

    let team2 = new Team(uuid(), null, null, 0)

    const newGame = new Game(
        _id,
        conversation.cid,
        GAME_STATE.LOBBY,
        false,
        team1,
        team2,
        [],
        [],
        game.max_rounds,
        getCurrentTimestamp(),
        GAME_UPDATE.DEFAULT,
        name
    )

    await mongo(COLLECTION).insertOne(newGame)
    const result = await mongo(COLLECTION).findOne({ _id })

    setTimeout(() => {
        GameFlow.sendMessage(result,
            `Bienvenue @${user.username}! Je suis le moderateur du jeu. Commence par inviter un autre joueur à jouer, vous devez être au moins deux.`,
            `Welcome @${user.username}! I am the game moderator. Start by inviting another player to join the game, you at least be two.`)
    }, 250)

    return toGame(result)
}

export async function update(game: IUpdateGameRequest): Promise<IGame> {

    await mongo(COLLECTION).findOneAndUpdate({ _id: game._id }, { $set: game })

    return toGame(await get(game._id))
}

export async function addPlayer(game_id: string, body: IAddPlayerRequest): Promise<IGame> {
    let game: IGame = await mongo(COLLECTION).findOne({ _id: game_id })

    const players = getPlayers(game)
    const uids = getUids(game)

    if (uids.length >= 4) {
        throw Error(JSON.stringify({ fr: "Le jeu contient déjà le nombre maximal de joueurs.", en: "This game is already full."}))
    }

    if (uids.includes(body.uid)) {
        throw Error(JSON.stringify({ fr: "Le joueur est déjà dans la partie.", en: "The player is already part of the game."}))
    }

    uids.push(body.uid)
    
    game = await balancePlayers(game, uids, players)

    await ConversationActions.addUser(game.conversation_id, body.uid)
    notifyConversationUpdate(uids, game.conversation_id)

    await mongo(COLLECTION).findOneAndUpdate({ _id: game_id }, { $set: game })

    const user = await UserActions.get(body.uid)

    GameFlow.sendMessage(game,
        `@${user.username} vient de joindre la salle d'attente.`,
        `@${user.username} joinded the lobby.`)
    
    return toGame(await get(game_id))
}


export async function removePlayer(ctx: any, game_id: string, body: IAddPlayerRequest): Promise<IGame> {
    console.log("GAME ID", game_id, body)

    let game: IGame = toGame(await mongo(COLLECTION).findOne({ _id: game_id }))

    const uids = getUids(game)
    ctx.uids = [...uids]

    await ConversationActions.removeUser(ctx, game.conversation_id, body.uid)
    notifyConversationUpdate(ctx.uids, game.conversation_id)

    const index = uids.findIndex((uid) => uid == body.uid)
    const uidsWithRemoved = [...uids.slice(0,index), ...uids.slice(index+1)]

    const areAllVirtual: boolean = uidsWithRemoved.every((uid) => uid.includes('virtual:'))

    if (!areAllVirtual && uidsWithRemoved.length > 0) {
        game = await balancePlayers(game, uidsWithRemoved, getPlayers(game))
        await mongo(COLLECTION).updateOne({ _id: game_id }, { $set: game })

        const user = await UserActions.get(body.uid)
        
        GameFlow.sendMessage(game,
            `@${user.username} vient de quitter la salle d'attente.`,
            `@${user.username} quitted the lobby.`)
    
        return toGame(await mongo(COLLECTION).findOne({ _id: game_id }))
    }

    await ConversationActions.destroy(ctx, game.conversation_id)
    EmitInRoom(
        ctx.uids,
        ConversationController.delete.one.response,
        { cid: game.conversation_id }
    )

    await mongo(COLLECTION).deleteOne({ _id: game_id })
    EmitInRoom(
        uids,
        GameController.delete.one.response,
        { _id: game_id }
    )

    return null
}

export async function cancel(game_id: string): Promise<void> {
    console.log("CANCEL GAME", game_id)

    let game: IGame = toGame(await mongo(COLLECTION).findOne({ _id: game_id }))
    const uids = getUids(game)

    await ConversationActions.destroy(null, game.conversation_id)
    EmitInRoom(
        uids,
        ConversationController.delete.one.response,
        { cid: game.conversation_id }
    )

    await mongo(COLLECTION).deleteOne({ _id: game_id })
    EmitInRoom(
        uids,
        GameController.delete.one.response,
        { _id: game_id }
    )

    EmitInRoom(
        uids,
        GameController.patch.cancel.response,
        { 
            _id: game_id,
            desc: {
                fr: "Un joueur a quitté la partie. Le jeu est annulé.",
                en: "A player exited the game. The game is now canceled."
            }
        }
    )
}

export async function cancelWithUser(uid: string): Promise<void> {
    console.log("CANCEL GAME BY USER ID", uid)

    const query = { 
        $and: [    
            { state: { $nin: [GAME_STATE.LOBBY, GAME_STATE.COMPLETED] } },
            {
                $or: [
                    { 'teamOne.playerOne.user_id': uid },
                    { 'teamOne.playerTwo.user_id': uid },
                    { 'teamTwo.playerOne.user_id': uid },
                    { 'teamTwo.playerTwo.user_id': uid }
                ]
            }
        ]
     }

    console.log(JSON.stringify(query))

    let games: IGame[] = DocumentsToArray((await mongo(COLLECTION).find(query).toArray()), toGame)

     console.log("GAMES TO CANCEL", games)

     for (let game of games) {
         await cancel(game._id)
     }

     let gamesInLobby: IGame[] = DocumentsToArray((await mongo(COLLECTION).find({ 
        $and: [
            { state: GAME_STATE.LOBBY },
            {
                $or: [
                    { 'teamOne.playerOne.user_id': uid },
                    { 'teamOne.playerTwo.user_id': uid },
                    { 'teamTwo.playerOne.user_id': uid },
                    { 'teamTwo.playerTwo.user_id': uid }
                ]
            }
        ]
    }).toArray()), toGame)

     console.log("GAMES IN LOBBY", gamesInLobby)

     for (let game of gamesInLobby) {
        await removePlayer({}, game._id, { _id: game._id, uid })
    }
    
}

export async function action(action: IAction): Promise<IDeleteGame> {

    let gf = new GameFlow()
    await gf.setGameById(action.game_id)
    await gf.parseAction(action)

    return gf.getGame()
}

export async function addVirtualPlayer(body: IAddVirtualPlayerRequest): Promise<IGame> {
    let game: IGame = await mongo(COLLECTION).findOne({ _id: body._id })
    
    let ids =  [
        "virtual:eaeacf6f-2292-4cd4-9b68-45b6c998fb4d",
        "virtual:8b1e9221-c884-4671-87f2-9e2149d0f8d4",
        "virtual:fca01daa-ef68-40c1-9832-70a84174e57c",
        "virtual:4cd389a3-be0e-43e9-94dd-f3cbeded5c4d",
        "virtual:a38c5544-933c-4bfe-af3a-0b1892bf8d07",
        "virtual:8a839f1e-5c2f-485f-87a5-9676bd733cf2",
    ]
    ids = ids.filter((id) => !getUids(game).includes(id))
    const id = ids[Math.floor(Math.random()*ids.length)]

    let user: IUser = await UserActions.getBasic(id)

    const players = getPlayers(game)
    const uids = getUids(game)

    if (uids.filter(uid => !uid.includes("virtual:")).length < 2) {
        throw Error("You must have at least two real players.")
    }

    uids.push(user.uid)
    
    game = await balancePlayers(game, uids, players)

    await ConversationActions.addUser(game.conversation_id, user.uid)
    notifyConversationUpdate(uids, game.conversation_id)

    await mongo(COLLECTION).updateOne({ _id: body._id }, { $set: game })

    GameFlow.sendMessage(game,
        `@${user.username} vient de joindre la salle d'attente.`,
        `@${user.username} joinded the lobby.`)
    
    return toGame(await mongo(COLLECTION).findOne({ _id: body._id }))
}

export async function destroy(ctx: Context, game_id: string): Promise<IDeleteGame> {

    let game: IGame = await mongo(COLLECTION).findOne({ _id: game_id })
    if (game && ctx) ctx.uids = getUids(game)

    await mongo(COLLECTION).deleteOne({ _id: game_id })

    return { _id: game_id }
}


/**
 * HELPERS
 */

const notifyConversationUpdate = async function (uids: string[], cid: string) {
    const conversation = await ConversationActions.get(cid)
    if (conversation) {
        EmitInRoom(
            uids,
            ConversationController.patch.removeUser.response,
            conversation
        )
    }
}

export const balancePlayers = async function(game: IGame, uids: string[], players): Promise<IGame> {
    uids = uids.sort((a, b) => a.includes("virtual:") ? b.includes("virtual:") ? 0 : 1 : -1)

    game.teamOne.playerOne = uids[0] ? await getOrCreatePlayer(uids[0], players) : null
    game.teamTwo.playerOne = uids[1] ? await getOrCreatePlayer(uids[1], players) : null
    game.teamOne.playerTwo = uids[2] ? await getOrCreatePlayer(uids[2], players) : null
    game.teamTwo.playerTwo = uids[3] ? await getOrCreatePlayer(uids[3], players) : null

    if (game.teamOne.playerOne && game.teamTwo.playerOne) {
        game.isReady = true
    }
    
    return game
}

const getOrCreatePlayer = async function (uid: string, players) {
    if (players[uid])
        return players[uid]
    
    let user: IUser = await UserActions.getBasic(uid)
    let playerUser: IPlayerUser = new PlayerUser(user.uid, user.username, user.profileImgUrl)
    
    return new Player(uuid(), user.uid, playerUser)
}

export const getPlayers = function(game: IGame) {
	let players = {}

	if (game.teamOne && game.teamOne.playerOne) players[game.teamOne.playerOne.user_id] = game.teamOne.playerOne
	if (game.teamOne && game.teamOne.playerTwo) players[game.teamOne.playerTwo.user_id] = game.teamOne.playerTwo
	if (game.teamTwo && game.teamTwo.playerOne) players[game.teamTwo.playerOne.user_id] = game.teamTwo.playerOne
	if (game.teamTwo && game.teamTwo.playerTwo) players[game.teamTwo.playerTwo.user_id] = game.teamTwo.playerTwo

	return players
}

export const getUids = function(game: IGame) {
	let uids = []

	if (game.teamOne && game.teamOne.playerOne) uids.push(game.teamOne.playerOne.user_id)
	if (game.teamOne && game.teamOne.playerTwo) uids.push(game.teamOne.playerTwo.user_id)
	if (game.teamTwo && game.teamTwo.playerOne) uids.push(game.teamTwo.playerOne.user_id)
	if (game.teamTwo && game.teamTwo.playerTwo) uids.push(game.teamTwo.playerTwo.user_id)

	return uids
}

export function toGame(game: IGame | any): IGame {
    return new Game(
        game._id,
        game.conversation_id,
        game.state,
        game.isReady,
        game.teamOne,
        game.teamTwo,
        game.actions,
        game.rounds,
        game.max_rounds,
        game.timestamp,
        game.updateAction,
        game.name
    )
}