import { Base, IBase } from "./Base";
import { ICanvas } from "./Canvas";
import { IGameImage } from "./GameImage";


export interface IRound extends IBase {
    _id: string,
    team: string,
    player1_guessing_id: string,
    player2_guessing_id: string,
    player_drawing_id: string,
    game_img: IGameImage,
    team_win_id: string,
    points_won: number,
    canvas: ICanvas,
    startTimestamp: number,
    endTimestamp: number,
    reply_team_id: string,
    reply_player1_guessing_id: string,
    reply_player2_guessing_id: string,
}

export class Round extends Base implements IRound, IBase {
    constructor(
        public _id: string,
        public team: string,
        public player1_guessing_id: string,
        public player2_guessing_id: string,
        public player_drawing_id: string,
        public game_img: IGameImage,
        public team_win_id: string,
        public points_won: number,
        public canvas: ICanvas,
        public startTimestamp: number = null,
        public endTimestamp: number = null,
        public reply_team_id: string = null,
        public reply_player1_guessing_id: string = null,
        public reply_player2_guessing_id: string = null,
    ){
        super()
    }
}