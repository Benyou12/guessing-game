import { IMessage } from "../Models/Message";
import { GAME_DIFFICULTY, DRAWING_MODES, WORD_LANG } from "../Models/GameImage";
import { ICanvas } from "../Models/Canvas";

export interface IGetGameImageRequest {
    _id: string
}

export interface ICreateGameImageRequest {
    hints: string[],
    word: string,
    difficulty: GAME_DIFFICULTY,
    svg_link?: string,
    canvas?: ICanvas,
    drawing_mode: DRAWING_MODES,
    lang: WORD_LANG
}

export interface IUpdateGameImageRequest {
    _id: string,
    hints?: string[],
    word?: string,
    difficulty?: GAME_DIFFICULTY,
    svg_link?: string,
    canvas?: ICanvas,
    drawing_mode?: DRAWING_MODES
}

export interface IGetImagesRequest {
    keyword: string
}

export interface IGetImagesResponse {
    filename: string,
    url: string,
    height: string,
    width: string,
    desc: string
}