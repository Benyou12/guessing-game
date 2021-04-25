import { Base, IBase } from "./Base";
import { ITeam } from "./Team";
import { ICanvas } from "./Canvas";

export enum GAME_ACTIONS {
    REQUEST_HINT = 0,
    GUESS_WORD = 1,
    ROUND_WON = 2,
    ADD_PLAYER = 3,
    START = 4,
    END = 5,
    START_ROUND = 6,
    END_ROUND = 7
}

export interface IActionWordGuess {
    word: string
}

/**
 * Action Interface
 */
export interface IAction extends IBase {
    game_id: string,
    type: GAME_ACTIONS,
    round_id: string,
    team_id: string,
    user_id: string,
    payload: IActionWordGuess
}

/**
 * Action Class
 */
export class Action extends Base implements IAction, IBase {
    constructor(
        public game_id: string,
        public type: GAME_ACTIONS,
        public round_id: string,
        public team_id: string,
        public user_id: string,
        public payload: IActionWordGuess
    ){
        super()
    }
}