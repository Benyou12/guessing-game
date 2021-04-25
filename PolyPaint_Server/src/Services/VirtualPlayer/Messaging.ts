import { IVirtualPlayer } from "../../Models/VirtualPlayer";
import MessageController from "../../Socket/Controllers/message"
import * as MessageActions from "../DataStore/Message"
import * as UserActions from "../DataStore/User"
import Texts from "./Texts"
import { EmitInRoom } from "../../Socket";
import { IGame } from "../../Models/Game";
import { IPlayer } from "../../Models/Player";
import { getUids } from "../DataStore/Game";
import { Message } from "../../Models/Message";
import { getCurrentTimestamp } from "../Time/GetTime";
import { getBasic } from "../DataStore/User";
import  * as uuid from "uuid/v4"
import * as VirtualPlayerActions from "../DataStore/VirtualPlayer"
import * as Handlebars from "handlebars"
import { ITeam } from "../../Models/Team";
import { ICreateMessageRequest } from "../../Http/Controllers/Message/interfaces";
import { IUser } from "../../Models/User";
import { IRound } from "../../Models/Round";
import { IBadge } from "../../Models/Badge";

interface IVPMessage {
    fr: string,
    en: string
}

class VPMessage implements IVPMessage {
    constructor(
        public fr: string = "",
        public en: string = ""
    ) { }

}

export enum MESSAGE_TYPE {
    GENERAL = "General",
    GREETINGS = "Greetings",
    WELCOME = "Welcome",
    VICTORY = "Victory",
    DEFEAT = "Defeat",
    GAME_VICTORY = "GameVictory",
    GAME_DEFEAT = "GameDefeat",
    HINT = "Hint"
}

export async function SendVirtualMessage(game: IGame, players: IPlayer[], type: MESSAGE_TYPE, id: string = "", hint: string = "") {
    let used = new Set()

    for (let player of players) {
        if (!player.user.uid.includes("virtual:"))
            return
        
        const vp: IVirtualPlayer = await VirtualPlayerActions.get(player.user_id)
        vp.user = player.user

        const message = type === MESSAGE_TYPE.GENERAL
            ? Texts[type][vp.personality][id]
            : getRandomMessage(Texts[type][vp.personality], used)
        
        const newMessage: IVPMessage = await parseMessage(game, vp, message, hint)

        if (type !== MESSAGE_TYPE.GENERAL) used.add(newMessage.fr)

        sendMessage(game, vp, newMessage)

        await wait(4000 * Math.random() + 500)
    }

}

function getRandomMessage(texts, used) {
    const filtered = texts.filter((text) => !used.has(text.fr))

    if (filtered.length == 0) {
        return texts[Math.floor(Math.random()*texts.length)]
    }

    return filtered[Math.floor(Math.random()*filtered.length)]
}

async function sendMessage(game: IGame, player: IVirtualPlayer, message: IVPMessage) {

    const finalMessage: ICreateMessageRequest = {
        cid: game.conversation_id,
        message: new Message(
            uuid(),
            JSON.stringify(message),
            getCurrentTimestamp(),
            await getBasic(player.user_id)
        )
    }

    await MessageActions.create(finalMessage)

    EmitInRoom(
        getUids(game),
        MessageController.post.create.response,
        finalMessage
    )
}

async function wait(duration: number) {
    return new Promise((resolve, reject) => {
        setTimeout(resolve, duration)
    })
}

/** 
 * Message parsing
 */

async function parseMessage(game: IGame, player: IVirtualPlayer, message: any, hint: string = ""): Promise<IVPMessage> {
    
    let context = await getContext(game, player, hint)

    console.log("CONTEXT", context)
    
    return {
        fr: applyTemplate(message(context).fr, context),
        en: applyTemplate(message(context).en, context)
    }
}

function applyTemplate(message: string, context): string {
    var template = Handlebars.compile(message)

    return template(context)
}

export interface IGameContext {
    me: IPlayerStats,
    partner: IPlayerStats,
    opponent1: IPlayerStats,
    opponent2: IPlayerStats,
    random: IPlayerStats,
    currentRound: IRound,
    hint: string
}

async function getContext(game: IGame, player: IVirtualPlayer, hint: string) {

    const { myTeam, otherTeam } = getTeams(game, player)
    const { partner, opponent1, opponent2 } = getOtherPlayers(myTeam, otherTeam)

    const currentRound = game.rounds.length > 0 ? game.rounds[game.rounds.length - 1] : null

    const otherStats = {
        partner: await getPlayerStats(partner, player),
        opponent1: await getPlayerStats(opponent1, player),
        opponent2: await getPlayerStats(opponent2, player),
    }

    const allPlayers = [ otherStats.partner, otherStats.opponent1, otherStats.opponent2 ]
    const random = allPlayers[Math.floor(Math.random()*3)]

    return {
        me: await getPlayerStats(myTeam.playerTwo, player),
        ...otherStats,
        random,
        currentRound,
        hint
    }
}

function getOtherPlayers(myTeam: ITeam, otherTeam: ITeam) {
    return {
        partner: myTeam.playerOne,
        opponent1: otherTeam.playerOne,
        opponent2: otherTeam.playerTwo
    }
}

function getTeams(game: IGame, player: IVirtualPlayer) {
    const isMyTeam: boolean = game.teamOne.playerOne.user_id == player.user_id || game.teamOne.playerTwo.user_id == player.user_id

    return {
        myTeam: isMyTeam ? game.teamOne : game.teamTwo,
        otherTeam: !isMyTeam ? game.teamOne : game.teamTwo
    }
}

export interface IGameSimple {
    points: number
    timestamp: number
}
export interface IPlayerStats {
    username: string,
    currentLevel: string,
    pointsToNextLevel: number,
    nextLevel: string,
    totalBadges: number,
    lastBadge: IBadge,
    games: {
        count: number,
        bestGame: IGameSimple,
        worstGame: IGameSimple,
        lastGame: IGameSimple,
        firstGame: IGameSimple,
        victories: number,
        failures: number,
        round_vitories: number,
        round_failures: number,
    },
    games_in_commun: {
        count: number,
        bestGame: IGameSimple,
        worstGame: IGameSimple,
        lastGame: IGameSimple,
        firstGame: IGameSimple,
        victories: number,
        failures: number,
    }
}

async function getPlayerStats(player: IPlayer, vp: IVirtualPlayer): Promise<IPlayerStats> {

    const user: IUser = await UserActions.get(player.user_id)
    const gamesInCommun = user.game_history.filter(({ names }) => {
        console.log("GAME IN COMMUN", names, vp.user.username, names.includes(vp.user.username))
        return names.includes(vp.user.username)
    } )

    let stats = {
        username: user.username,
        currentLevel: null,
        pointsToNextLevel: null,
        nextLevel: null,
        totalBadges: null,
        lastBadge: null,
        games: {
            count: user.game_stats.total_games_played,
            bestGame: null,
            worstGame: null,
            lastGame: null,
            firstGame: null,
            victories: 0,
            failures: 0,
            round_vitories: user.game_stats.victories,
            round_failures: user.game_stats.failures,
        },
        games_in_commun: {
            count: gamesInCommun.length,
            bestGame: null,
            worstGame: null,
            lastGame: null,
            firstGame: null,
            victories: 0,
            failures: 0,
        }
    }

    setGameStats(stats.games, user.game_history)
    setGameStats(stats.games_in_commun, gamesInCommun)
    
    return stats
}

function setGameStats(games, history) {

    for (let game of history) {

        if (!games.bestGame || games.bestGame.points < game.myTeamResult) {
            games.bestGame = { points: game.myTeamResult, timestamp: game.timestamp }
        }

        if (!games.worstGame || games.bestGame.worstGame > game.myTeamResult) {
            games.worstGame = { points: game.myTeamResult, timestamp: game.timestamp }
        }

        if (!games.firstGame) {
            games.worstGame = { points: game.myTeamResult, timestamp: game.timestamp }
        }

        games.lastGame = { points: game.myTeamResult, timestamp: game.timestamp }

        if (game.myTeamResult > game.otherTeamResult) {
            games.victories += 1
        }

        if (game.myTeamResult < game.otherTeamResult) {
            games.failures += 1
        }

    }

}