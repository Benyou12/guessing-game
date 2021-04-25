import { IPosition, IColor, CAP } from "../../../Models/Canvas";

export interface IUpdateStroke {
    _id: string,
    canvas_id: string,
    size: number,
    color: IColor,
    cap: CAP,
    coordinates: IPosition[],
    toDelete?: boolean
}