import * as uuid from "uuid/v4"
import { mongo } from "./MongoDB/index"
import { Canvas, ICanvas } from "../../Models/Canvas";


export async function get(_id: string): Promise<ICanvas> {
    const data = await mongo("Canva").findOne({ _id })

    return toCanvas(data)
}

export async function create(uids: string[]): Promise<ICanvas> {

    const _id = uuid()
    const newGame = new Canvas(
        _id,
        [],
        uids
    )

    await mongo("Canva").insertOne(newGame)
    const data = await mongo("Canva").findOne({ _id })

    return toCanvas(data)
}


export function toCanvas(canvas: ICanvas): ICanvas {
    return new Canvas(
        canvas._id,
        [],
        canvas.uids
    )
}