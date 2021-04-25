import { Base, IBase } from "./Base";
import { ITeam } from "./Team";
import { IAction } from "./Action";
import { IRound } from "./Round";
import { getCurrentTimestamp } from "../Services/Time/GetTime";

export enum GAME_STATE {
    LOBBY = 0,
    SCOREBOARD = 1,
    ROUND = 2,
    REPLY = 3,
    COMPLETED = 4
}

export enum GAME_UPDATE {
    DEFAULT = 0,
    CREATED = 1,
    UPDATED = 2,
    USER_JOIN = 3,
    USER_QUIT = 4,
    VIRTUAL_JOIN = 5,
    ACTION_FLOW = 6
}

export interface IGame extends IBase {
    _id: string,
    conversation_id: string,
    state: GAME_STATE,
    isReady: boolean,
    teamOne: ITeam,
    teamTwo: ITeam,
    actions?: IAction[],
    rounds?: IRound[],
    max_rounds: number,
    timestamp: number,
    updateAction: GAME_UPDATE,
    name?: string
}

export class Game extends Base implements IGame, IBase {
    constructor(
        public _id: string,
        public conversation_id: string,
        public state: GAME_STATE = GAME_STATE.LOBBY,
        public isReady: boolean,
        public teamOne: ITeam,
        public teamTwo: ITeam,
        public actions: IAction[] = [],
        public rounds: IRound[] = [],
        public max_rounds: number = 4,
        public timestamp: number = getCurrentTimestamp(),
        public updateAction: GAME_UPDATE = GAME_UPDATE.DEFAULT,
        public name: string = ""
    ){
        super()
    }
}