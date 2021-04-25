import { ITeam } from "../Models/Team";
import { IAction } from "../Models/Action";
import { IRound } from "../Models/Round";
import { ICanvas } from "../Models/Canvas";
import { GAME_STATE } from "../Models/Game";
import { IGameImage } from "../Models/GameImage";

export interface IGetGameRequest {
    game_id: string
}

export interface ICreateGameRequest {
    uid: string,
    actions?: IAction[],
    max_rounds?: number
}

export interface IUpdateGameRequest {
    _id: string,
    conversation_id?: string,
    state?: GAME_STATE,
    isFinished?: boolean,
    isReady?: boolean,
    teamOne?: ITeam,
    teamTwo?: ITeam,
    actions?: IAction[],
    rounds?: IRound[]
}

export interface ICreateRoundRequest {
    team: string,
    player1_guessing_id: string,
    player2_guessing_id: string,
    player_drawing_id: string,
    game_img: IGameImage,
    team_win_id: string,
    points_won: number,
    canvas: ICanvas
}

export interface IGetRoundsRequest {
    game_id: string
}

export interface IGetRoundRequest {
    game_id: string,
    round_id: string
}

export interface IAddPlayerRequest {
    _id: string,
    uid: string
}

export interface IAddVirtualPlayerRequest {
    _id: string
}

export interface IDeleteGame {
    _id: string
}

export interface ICancelGame {
    _id: string
    desc: {
        fr: string,
        en: string
    }
}