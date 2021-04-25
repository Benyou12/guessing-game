import { DocumentsToArray } from "./MongoDB/utils"
import * as uuid from "uuid/v4"
import { IGameImage, GameImage } from "../../Models/GameImage";
import { ICreateGameImageRequest, IUpdateGameImageRequest } from "../../Interfaces/GameImage";
import { mongo } from "./MongoDB/index"
import game from "../../Socket/Controllers/game";

const COLLECTION = "GameImage";

export async function getAll(): Promise<Array<IGameImage>> {
    const data = await mongo(COLLECTION).find().toArray()

    return DocumentsToArray<IGameImage>(data, toGameImage)
}

export async function getRandom(prevImgIds: string[]): Promise<IGameImage> {
    const data = await mongo(COLLECTION).aggregate([
        { $match: { _id: { $nin : prevImgIds } } },
        { $sample: { size: 1 } }
    ]).toArray()

    console.log("GET RANDOM IMG", data)

    return toGameImage(data[0])
}

export async function get(_id: string): Promise<IGameImage> {
    const data = await mongo(COLLECTION).findOne({ _id })

    return toGameImage(data)
}

export async function create(gameImage: ICreateGameImageRequest): Promise<IGameImage> {

    const _id = uuid()
    const newGameImage = new GameImage(
        _id,
        gameImage.hints,
        gameImage.word,
        gameImage.difficulty,
        gameImage.svg_link,
        gameImage.canvas,
        gameImage.drawing_mode,
        gameImage.lang
    )

    await mongo(COLLECTION).insertOne(newGameImage)
    const data = await mongo(COLLECTION).findOne({ _id })

    return toGameImage(data)
}

export async function update(gameImage: IUpdateGameImageRequest): Promise<IGameImage> {

    await mongo(COLLECTION).update({ _id: gameImage._id }, { $set: gameImage })
    const data = await mongo(COLLECTION).findOne({ _id: gameImage._id })

    return toGameImage(data)
}

export function toGameImage(gameImage: GameImage): IGameImage {
    return new GameImage(
        gameImage._id,
        gameImage.hints,
        gameImage.word,
        gameImage.difficulty,
        gameImage.svg_link,
        gameImage.canvas,
        gameImage.drawing_mode,
        gameImage.lang
    )
}