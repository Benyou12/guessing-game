import { IGame, GAME_STATE, Game } from "../../Models/Game"
import * as GameDB from "../DataStore/Game"
import * as GameImageDB from "../DataStore/GameImage"
import * as UserDB from "../DataStore/User"
import * as CanvasDB from "../DataStore/Canvas"
import * as MessageDB from "../DataStore/Message"
import { IAction, GAME_ACTIONS } from "../../Models/Action"
import { IPlayer, PLAYER_ROLE, Player } from "../../Models/Player"
import { IRound, Round } from "../../Models/Round"
import { ITeam } from "../../Models/Team"
import { Canvas } from "../../Models/Canvas"
import uuid = require("uuid")
import { getCurrentTimestamp } from "../Time/GetTime"
import { IUser, User } from "../../Models/User"
import VirtualDrawing, { drawingEvent } from "../VirtualPlayer/Drawing"
import AddBadge from "../Gamification/index"
import addBadge from "../Gamification/index"
import { AddVirtalPlayer, FirstGame, Plus10Games, Plus1Hour } from "../Gamification/Badges"
import { IBadge, IUserBadge } from "../../Models/Badge"
import { EmitInRoom } from "../../Socket"
import GameController, { removeCanvasStrokes } from "../../Socket/Controllers/game"
import { SendVirtualMessage, MESSAGE_TYPE } from "../VirtualPlayer/Messaging"
import MessageController from "../../Socket/Controllers/message"
import { Message } from "../../Models/Message"
import { ICreateMessageRequest } from "../../Http/Controllers/Message/interfaces"
const levenshtein = require('js-levenshtein');

export default class GameFlow {
    private game: IGame = null

    public async setGame(game: IGame) {
        this.game = game
    }

    public async setGameById(game_id: string) {
        this.game = await GameDB.get(game_id)
    }

    public getGame(): IGame {
        return this.game
    }

    /**
     * Actions Handlers
     */

    public async parseAction(action: IAction): Promise<IGame> {

        //TODO: Send action to Virtual Player Message
        let newGame = this.game

        switch(action.type) {
            case GAME_ACTIONS.START:
                newGame = await this.startAction(action.user_id)
                break
            case GAME_ACTIONS.START_ROUND:
                newGame = await this.startRoundAction()
                break
            case GAME_ACTIONS.GUESS_WORD:
                newGame = await this.guessWordAction(action.payload.word)
                break
            default:
                throw "Unsupported Game Action"
        }

        this.game.actions.push(action)
        await this.save()

        return newGame

    }

    private async startAction(uid: string) {

        if (this.game.state !== GAME_STATE.LOBBY) {
            throw JSON.stringify({
                fr: "Le jeu doit commencer à partir d'un lobby",
                en: "The game must start from a lobby."
            })
        }

        const uids = GameDB.getUids(this.game)
        const virtualPlayers = this.getVirtualPlayers()

        const usersnames = this.getPlayersInGame().map(player => player.user.username)
        GameFlow.sendMessage(this.game,
            `Le jeu commence!`,
            `The game is starting!`)

        if (virtualPlayers.length > 0 || uids.length < 4) {
            this.addVirtualPlayerBadge()
        }

        if (virtualPlayers.length > 2) {
            throw JSON.stringify({
                fr: "Vous ne pouvez pas avoir plus de 2 joueurs virtuels dans la partie",
                en: "You cannont have more than 2 virtual players in a game."
            })
        }

        if (uids.length < 4) {
            for (let i = 0; i < (4 - uids.length + 1); i++) {
                this.game = await GameDB.addVirtualPlayer({ _id: this.game._id })
            }
        }

        SendVirtualMessage(this.game, this.getVirtualPlayers(), MESSAGE_TYPE.GREETINGS)
        setTimeout(() => {
            SendVirtualMessage(this.game, this.getVirtualPlayers(), MESSAGE_TYPE.WELCOME)
        }, 2000)
        

        return this.newRoundAction()
    }

    private async newRoundAction() {

        // if (![GAME_STATE.LOBBY, GAME_STATE.ROUND, GAME_STATE.REPLY].includes(this.game.state)) {
        //     throw JSON.stringify({
        //         fr: "Le jeu n'est pas dans un état valide pour créer une nouvelle round.",
        //         en: "The game is not in a valid state to start a new round."
        //     })
        // }

        if (this.getLastRound()) {
            await this.setRoundStats(this.game.rounds.length == this.game.max_rounds)
        }

        if (this.game.rounds.length == this.game.max_rounds) {
            await this.setGameStats()

            this.endOfGameBadges()

            this.game.state = GAME_STATE.COMPLETED

            if (this.game.teamOne.score == this.game.teamTwo.score) {

                GameFlow.sendMessage(this.game,
                    `Bravo! C'est une egalité. Je vais donner une victorire aux deux équipes.`,
                    `Congrats! It's an equality. I will give a victory to both teams.`)

                SendVirtualMessage(this.game, [this.game.teamOne.playerTwo], MESSAGE_TYPE.GAME_VICTORY)
                SendVirtualMessage(this.game, [this.game.teamTwo.playerTwo], MESSAGE_TYPE.GAME_VICTORY)
            } else if (this.game.teamOne.score > this.game.teamTwo.score) {
                SendVirtualMessage(this.game, [this.game.teamOne.playerTwo], MESSAGE_TYPE.GAME_VICTORY)
                SendVirtualMessage(this.game, [this.game.teamTwo.playerTwo], MESSAGE_TYPE.GAME_DEFEAT)
            } else {
                SendVirtualMessage(this.game, [this.game.teamTwo.playerTwo], MESSAGE_TYPE.GAME_VICTORY)
                SendVirtualMessage(this.game, [this.game.teamOne.playerTwo], MESSAGE_TYPE.GAME_DEFEAT)
            }

            console.log("STATE COMPLETED")

            this.deleteGame()

            return this.game
        }

        const nextTeam = this.getNextTeam()
        const { guessing, drawing } = this.getRoundRoles(nextTeam)
        const canvas  = await CanvasDB.create(GameDB.getUids(this.game))
        const gameImage = await GameImageDB.getRandom(this.game.rounds.map(({_id}) => _id))
        this.setPlayerRoles(guessing, drawing)

        const scoreboardTimeout = 10 * 1000

        const round: IRound = new Round(
            uuid(),
            nextTeam._id,
            guessing._id,
            null,
            drawing._id,
            gameImage, 
            null,
            0,
            canvas,
            getCurrentTimestamp() + scoreboardTimeout
        )

        this.game.rounds.push(round)
        this.game.state = GAME_STATE.SCOREBOARD

        GameFlow.sendMessage(this.game,
            `@${drawing.user.username} c'est ton tour de dessiner et @${guessing.user.username} tu devines.`,
            `@${drawing.user.username} it's your turn to draw and @${guessing.user.username} you are guessing.`)

        if (GameFlow.isVirtual(drawing)) {
          setTimeout(() => {
            GameFlow.sendMessage(this.game,
                `@${guessing.user.username} envoie "J'ai besoin d'un indice" ou "Aide-moi!" si t'as besoin d'aide durant ce tour.`,
                `@${guessing.user.username} send "I need a hint" or "Help-me!" if you need help during this round.`)
          }, 250)  
        } 

        

        
        setTimeout(() => {
            console.log("MOVE TO GAME AFTER", scoreboardTimeout)
            this.startRoundAction()
        }, scoreboardTimeout)

        return this.game
    }

    private async startRoundAction() {
        const team = this.getLastRound().team == this.game.teamOne._id ? this.game.teamOne : this.game.teamTwo

        if (GameFlow.isVirtual(team.playerTwo)) {
            const vd = new VirtualDrawing(this.game, this.getLastRound().game_img)
            vd.startDrawing(250)
        }

        this.game.state = GAME_STATE.ROUND
        
        EmitInRoom(
            GameDB.getUids(this.game),
            GameController.patch.action.response,
            removeCanvasStrokes(this.game)
        )

        this.save()

        return this.game
    }

    private async guessWordAction(word: string) {

        console.log("WORD GUESSED", word, this.game.state)

        // if (![GAME_STATE.ROUND, GAME_STATE.REPLY].includes(this.game.state)) {
        //     throw JSON.stringify({
        //         fr: "État précédent invalide pour deviner un mot.",
        //         en: "Previous state invalid to guess a word."
        //     })
        // }

        function normalize(word: string) {
            return word.toLocaleLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "")
        }

        const roundWord = normalize(this.getLastRound().game_img.word)
        const normalizedWord = normalize(word)
        const distance = levenshtein(normalizedWord, roundWord)
        const ratio = (roundWord.length - distance) / roundWord.length

        console.log("WORD GUESSING", roundWord, normalizedWord, distance, ratio)

        if (ratio > 0.8) {
            let currTeamId = this.getLastRound().team
            let lastRoundIndex = this.getLastRoundIndex()

            let team = this.game.teamOne._id === currTeamId ? this.game.teamOne : this.game.teamTwo
            if (this.game.state === GAME_STATE.REPLY) {
                team = this.game.teamOne._id === currTeamId ? this.game.teamTwo : this.game.teamOne
            }

            if (distance == 0) {
                GameFlow.sendMessage(this.game,
                    `Bonne réponse! Le mot à deviner était ${this.getLastRound().game_img.word}.`,
                    `Good answer! The word to guess was ${this.getLastRound().game_img.word}.`)
            } else {
                GameFlow.sendMessage(this.game,
                    `Assez proche, je vais l'accepter! Le mot à deviner était ${this.getLastRound().game_img.word}.`,
                    `Close enough, I will accept it! The word to guess was ${this.getLastRound().game_img.word}.`)
            }
            
                
            this.game.rounds[lastRoundIndex].team_win_id = team._id
            this.game.rounds[lastRoundIndex].points_won += 10

            if (this.game.teamOne._id === team._id) {
                this.game.teamOne.score += 10
                SendVirtualMessage(this.game, [this.game.teamOne.playerTwo], MESSAGE_TYPE.VICTORY)
                SendVirtualMessage(this.game, [this.game.teamTwo.playerTwo], MESSAGE_TYPE.DEFEAT)
            } else {
                this.game.teamTwo.score += 10
                SendVirtualMessage(this.game, [this.game.teamTwo.playerTwo], MESSAGE_TYPE.VICTORY)
                SendVirtualMessage(this.game, [this.game.teamOne.playerTwo], MESSAGE_TYPE.DEFEAT)
            }

            drawingEvent.emit(`drawingEvent-${this.getLastRound().game_img._id}`)

        } else {
            if (this.game.state === GAME_STATE.ROUND) {
                drawingEvent.emit(`drawingEvent-${this.getLastRound().game_img._id}`)

                GameFlow.sendMessage(this.game,
                    `Hum, this wasn't the answer I expected.`,
                    `Hum, this wasn't the answer I expected.`)

                return this.giveReplyAction()
            } else {
                GameFlow.sendMessage(this.game,
                    `Personne n'a trouvé l'a trouvé. Le mot était ${this.getLastRound().game_img.word}.`,
                    `Nobody found it! The word to guess was ${this.getLastRound().game_img.word}.`)
            }
        }

        return this.newRoundAction()
    }

    private async giveReplyAction() {
        this.getLastRound().reply_team_id = this.getNextTeam()._id
        this.getLastRound().reply_player1_guessing_id = this.getNextTeam().playerOne._id
        this.getLastRound().reply_player2_guessing_id = this.getNextTeam().playerTwo._id

        this.setPlayerRolesReply(
            this.getNextTeam().playerOne,
            this.getNextTeam().playerTwo
        )

        this.game.state = GAME_STATE.REPLY
        
        const usernames = [
            this.getNextTeam().playerOne.user.username,
            this.getNextTeam().playerTwo.user.username
        ]

        GameFlow.sendMessage(this.game,
            `@${GameFlow.usernameToString(usernames)} vous avez un droit de réplique! C'est votre chance de briller!`,
            `@${GameFlow.usernameToString(usernames)} you can reply! It's your chance to shine!`)

        this.save()

        return this.game
    }

    private async setRoundStats(isLastRound: boolean) {
        const round = this.getLastRound()
        const noWinner = !round.team_win_id
        const teamWon = round.team_win_id == this.game.teamOne._id ? this.game.teamOne : this.game.teamTwo
        const teamLost = round.team_win_id == this.game.teamOne._id ? this.game.teamTwo : this.game.teamOne

        round.endTimestamp = getCurrentTimestamp()

        let roundTime = round.endTimestamp - round.startTimestamp

        await this.addPlayerStats(teamWon.playerOne, !noWinner, roundTime, round, isLastRound)
        await this.addPlayerStats(teamWon.playerTwo, !noWinner, roundTime, round, isLastRound)
        await this.addPlayerStats(teamLost.playerOne, false, roundTime, round, isLastRound)
        await this.addPlayerStats(teamLost.playerTwo, false, roundTime, round, isLastRound)
    }

    private async addPlayerStats(player: IPlayer, won: boolean, duration: number, round: IRound, isLastRound: boolean) {
        const user: IUser = await UserDB.get(player.user_id)

        const totalRounds = (user.game_stats.rounds_played || 0) + 1
        const totalTime = user.game_stats.rounds_played + duration

        let stats = {
            rounds_played: totalRounds,
            victories:  user.game_stats.victories + (won ? 1 : 0),
            failures: user.game_stats.failures + (!won ? 1 : 0),
            rounds_avg_time: Math.round(totalTime/totalRounds),
            total_time_played: totalTime,
            total_games_played: user.game_stats.total_games_played
        }
        
        if (isLastRound) {
            stats.total_games_played = user.game_stats.total_games_played + 1
        }

        await UserDB.update({
            uid: player.user_id,
            game_stats: stats,
            gamification: {
                ...user.gamification,
                points: user.gamification.points + (won ? 1 : 0)
            }
        })
    }

    private async setGameStats() {
        await this.addFinalGameStats(this.game.teamOne.playerOne, this.game.teamOne, this.game.teamTwo)
        await this.addFinalGameStats(this.game.teamOne.playerTwo, this.game.teamOne, this.game.teamTwo)
        await this.addFinalGameStats(this.game.teamTwo.playerOne, this.game.teamOne, this.game.teamTwo)
        await this.addFinalGameStats(this.game.teamTwo.playerTwo, this.game.teamOne, this.game.teamTwo)   
    }

    private async addFinalGameStats(player: IPlayer, teamOne: ITeam, teamTwo: ITeam) {
        const players = GameDB.getPlayers(this.game)
        const uids = GameDB.getUids(this.game)
        
        const myTeam = this.isMyTeam(teamOne, player._id) ? teamOne : teamTwo
        const otherTeam = this.isMyTeam(teamOne, player._id) ? teamTwo : teamOne

        await UserDB.createGameStatHistory(player.user_id, {
            game_id: this.game._id,
            timestamp: this.game.timestamp,
            names: uids.map((uid) => players[uid].user.username),
            myTeamResult: myTeam.score,
            otherTeamResult: otherTeam.score,
            name: this.game.name
        })

        const pointsForWin = myTeam.score >= otherTeam.score && myTeam.score > 0 ? 2 : 0

        if (pointsForWin) {

            const user = await UserDB.get(player.user.uid)

            await UserDB.update({
                uid: player.user.uid, 
                gamification: {
                    ...user.gamification,
                    points: user.gamification.points + pointsForWin
                }
            })
        }
        

    }

    private deleteGame(): void {
        const deleteIn = 90 * 1000

        GameFlow.sendMessage(this.game,
            `Merci d'avoir joué!`,
            `Thanks for playing!`)

        setTimeout(() => {
            GameFlow.sendMessage(this.game,
                `Le jeu et la conversation seront supprimés dans 90 secondes. Vous pouvez créer une nouvelle conversion dans la section "Messages".`,
                `The game and the conversation will be delete in 90 seconds. You can create a new conversation in the "Messages" section.`)
        }, 1000)

        setTimeout(async () => {
            await GameDB.destroy(null, this.game._id)
        }, deleteIn)
        
    }


    /**
     * Actions Utils
     */
    private static isVirtual(player: IPlayer): Boolean {
        return !!player && player.user_id.includes("virtual:")
    }

    private getVirtualPlayers(): IPlayer[] {
        let virtualPlayers = []

        if (GameFlow.isVirtual(this.game.teamOne.playerOne)) 
            virtualPlayers.push(this.game.teamOne.playerOne)

        if (GameFlow.isVirtual(this.game.teamOne.playerTwo)) 
            virtualPlayers.push(this.game.teamOne.playerTwo)

        if (GameFlow.isVirtual(this.game.teamTwo.playerOne)) 
            virtualPlayers.push(this.game.teamTwo.playerOne)

        if (GameFlow.isVirtual(this.game.teamTwo.playerTwo)) 
            virtualPlayers.push(this.game.teamTwo.playerTwo)

        return virtualPlayers
    }

    private getRoundRoles(team: ITeam): { guessing: IPlayer, drawing: IPlayer } {

        if (GameFlow.isVirtual(team.playerTwo)) {
            return {
                guessing: team.playerOne,
                drawing: team.playerTwo
            }
        }

        const lastRound: IRound = this.getLastTeamRound(team._id)

        if (lastRound) {
            return {
                guessing: lastRound.player_drawing_id === team.playerOne._id ? team.playerOne : team.playerTwo,
                drawing: lastRound.player_drawing_id === team.playerOne._id ? team.playerTwo : team.playerOne
            }
        }

        return {
            guessing: team.playerTwo,
            drawing: team.playerOne
        }
    }

    private setPlayerRoles(guessing, drawing) {
        this.setPlayerRole(this.game.teamOne.playerOne, guessing, drawing)
        this.setPlayerRole(this.game.teamOne.playerTwo, guessing, drawing)
        this.setPlayerRole(this.game.teamTwo.playerOne, guessing, drawing)
        this.setPlayerRole(this.game.teamTwo.playerTwo, guessing, drawing)
    }

    private setPlayerRole (player: Player, guessing: Player, drawing: Player) {
        if (player._id == guessing._id) {
            player.role = PLAYER_ROLE.GUESS
        } else if (drawing && player._id == drawing._id) {
            player.role = PLAYER_ROLE.DRAW
        } else {
            player.role = PLAYER_ROLE.NONE
        }
    }

    private setPlayerRolesReply(guessing1, guessing2) {
        this.setPlayerRoleReply(this.game.teamOne.playerOne, guessing1, guessing2)
        this.setPlayerRoleReply(this.game.teamOne.playerTwo, guessing1, guessing2)
        this.setPlayerRoleReply(this.game.teamTwo.playerOne, guessing1, guessing2)
        this.setPlayerRoleReply(this.game.teamTwo.playerTwo, guessing1, guessing2)
    }

    private setPlayerRoleReply (player: Player, guessing1: Player, guessing2: Player) {
        if (player._id == guessing1._id || player._id == guessing2._id) {
            player.role = PLAYER_ROLE.GUESS
        } else {
            player.role = PLAYER_ROLE.NONE
        }
    }

    private getLastTeamRound(teamId: string): IRound {
        return this.game.rounds.reverse().find(({ team }) => team === teamId)
    }

    private getLastRound(): IRound {
        return this.game.rounds[this.game.rounds.length - 1]
    }

    private getLastRoundIndex(): number {
        return this.game.rounds.length - 1
    }

    private getNextTeam(): ITeam {
        if (this.game.rounds.length === 0)
            return this.game.teamOne

        return this.getLastRound().team === this.game.teamOne._id ? 
            this.game.teamTwo : this.game.teamOne
    }

    private isMyTeam(team: ITeam, player_id: string): boolean {
        return team.playerOne._id == player_id || team.playerTwo._id == player_id
    }

    private getPlayersInGame(): IPlayer[] {
        const uids = GameDB.getUids(this.game)
        const players = GameDB.getPlayers(this.game)

        return uids.filter((uid: string) => !GameFlow.isVirtual(players[uid])).map((uid: string) => players[uid])
    }

    private async save(): Promise<void> {
        await GameDB.update(this.game)
    }

    /**
     * Badges help
     */

     private addVirtualPlayerBadge() {
        const players: IPlayer[] = this.getPlayersInGame()

        players.forEach((player: IPlayer) => {
            AddBadge(AddVirtalPlayer, this.game._id, player.user_id)
        })
     }

     private async endOfGameBadges() {
        const players: IPlayer[] = this.getPlayersInGame()

        for (let player of players) {
            const user = await UserDB.get(player.user_id)

            if (user.game_stats.total_games_played === 1) {
                AddBadge(FirstGame, this.game._id, player.user_id)
            }

            console.log("END GAME BADGE 10: ", user.game_stats.total_games_played, user.game_stats.total_games_played % 10 == 0)
            if (user.game_stats.total_games_played > 0 && user.game_stats.total_games_played % 10 == 0) {
                AddBadge(Plus10Games, this.game._id, player.user_id)
            }

            
            const nbrOfHourBadges = user.gamification.badges.filter((badge: IUserBadge) => badge.badge.badge_id === "plus_1_hour").length

            console.log("HOUR BADGES", nbrOfHourBadges, Math.floor((user.game_stats.total_time_played / 600))  > nbrOfHourBadges)
            if (Math.floor((user.game_stats.total_time_played / 600))  > nbrOfHourBadges) {
                AddBadge(Plus1Hour, this.game._id, player.user_id)
            }
        }
     }

     /**
      * Game Moderator
      */
    
    public static async sendMessage(game: IGame, fr: string, en: string) {

        console.log("SEND MODERATOR MESSAGE", game.conversation_id, fr)

        const user = new User(
            "moderator",
            "game",
            "moderator",
            "moderator@polychat.com",
            "moderator",
            "")

        const message: ICreateMessageRequest = {
                cid: game.conversation_id,
                message: {
                    text: JSON.stringify({ fr, en }),
                    user: {
                        uid: user.uid
                    }
                }
            }

        const created = await MessageDB.create(message, user)

        EmitInRoom(
            GameDB.getUids(game),
            MessageController.post.create.response,
            created
        )
    }

    public static usernameToString(usernames: string[]) {

        if (usernames.length === 0)
            return ""
        
        if (usernames.length === 1)
            return `${usernames[0]}`

        const last = usernames.pop()
        return `${usernames.join(", ")} & ${last}`

    }



    /**
     * Messaging reactions
     */

    public parseMessage(uid: string, message: string) {

        console.log("PARSING MESSAGE", uid, message)

        // Asking hint
        if (['indice', 'aide', 'hint', 'help'].some((check) => message.toLocaleLowerCase().includes(check))) {
            console.log("NEED A HINT!")
            this.askHintAction(uid)
        }

        // Talking to virtual player
        if (GameFlow.isVirtual(this.game.teamOne.playerTwo) && message.includes(this.game.teamOne.playerTwo.user.username)) {
            //TODO: Reply to message
        }

        if (GameFlow.isVirtual(this.game.teamTwo.playerTwo) && message.includes(this.game.teamTwo.playerTwo.user.username)) {
            //TODO: Reply to message
        }

    }

    private async askHintAction(uid: string) {

        if (!this.getLastRound().reply_team_id) {

            console.log("IS VALID TO GET HINT")

            const players = GameDB.getPlayers(this.game)
            const player: IPlayer = players[uid]

            console.log("PLAYER EXIST", !!player, player && player.user.username)

            if(
                GameFlow.isVirtual(this.game.teamOne.playerTwo) && 
                player &&
                this.getLastRound().player1_guessing_id == player._id &&
                this.getLastRound().player_drawing_id == this.game.teamOne.playerTwo._id &&
                this.game.teamOne.playerTwo.user_id !== uid
            ) {
                this.sendHint(this.game.teamOne.playerTwo)
            }

            if(
                GameFlow.isVirtual(this.game.teamTwo.playerTwo) && 
                player &&
                this.getLastRound().player1_guessing_id == player._id &&
                this.getLastRound().player_drawing_id == this.game.teamTwo.playerTwo._id &&
                this.game.teamOne.playerTwo.user_id !== uid
            ) {
                this.sendHint(this.game.teamTwo.playerTwo)
            }
        }

    }

    private sendHint(player: IPlayer) {
        const hint = this.game.rounds[this.getLastRoundIndex()].game_img.hints.shift()
        this.save()

        console.log("HINT TO SEND", hint)

        if (hint) {
            console.log("SENDING HINT", hint)
            SendVirtualMessage(this.game, [player], MESSAGE_TYPE.HINT, null, hint)
        } else {
            console.log("NO MORE HINTS", this.game.rounds.length)
            SendVirtualMessage(this.game, [player], MESSAGE_TYPE.GENERAL, "noMoreHint", hint)
        }

    }
    

}