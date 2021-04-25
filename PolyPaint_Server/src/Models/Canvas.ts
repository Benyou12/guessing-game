import { Base, IBase } from "./Base"


export enum CAP {
    SQUARE = 0,
    ROUND = 1
}

/**
 * Canvas Interfaces
 */
export interface IPosition {
    _id: string,
    x: number,
    y: number
}

export interface IColor {
    _id: string,
    r: number,
    g: number,
    b: number,
    a: number
}

export interface IStroke extends IBase {
    _id: string,
    canvas_id: string,
    size: number,
    color: IColor,
    cap: CAP,
    coordinates: IPosition[],
    toDelete?: boolean
}

export interface ICanvas extends IBase {
    _id: string,
    strokes: IStroke[]
    uids: string[] // TEMP: For testing only
}

/**
 * Canvas Classes
 */
export class Stroke extends Base implements IStroke, IBase {
    constructor(
        public _id: string,
        public canvas_id: string,
        public size: number,
        public color: IColor,
        public cap: CAP,
        public coordinates: IPosition[],
        public toDelete: boolean = false,
    ){
        super()
    }
}

export class Canvas extends Base implements ICanvas, IBase {
    constructor(
        public _id: string,
        public strokes: IStroke[],
        public uids: string[] // TEMP: For testing only
    ){
        super()
    }
}