import { Base, IBase } from "./Base";
import { ICanvas } from "./Canvas";

export enum WORD_LANG {
    FR = 0,
    EN = 1
}

export enum GAME_DIFFICULTY {
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}

export enum DRAWING_MODES {
    RANDOM,
    CENTERED,
    PANORAMIC
}

export interface IGameImage extends IBase {
    _id: string,
    hints: string[],
    word: string,
    difficulty: GAME_DIFFICULTY,
    svg_link?: string,
    canvas?: ICanvas,
    drawing_mode: DRAWING_MODES,
    lang?: WORD_LANG
}

export class GameImage extends Base implements IGameImage, IBase {
    constructor(
        public _id: string,
        public hints: string[],
        public word: string,
        public difficulty: GAME_DIFFICULTY,
        public svg_link: string,
        public canvas: ICanvas,
        public drawing_mode: DRAWING_MODES,
        public lang: WORD_LANG = WORD_LANG.FR
    ){
        super()
    }
}