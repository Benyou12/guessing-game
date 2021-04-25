import { DocumentsToArray } from "./MongoDB/utils"
import * as uuid from "uuid/v4"
import { ICreateVirtualPlayer, IUpdateVirtualPlayer } from "../../Interfaces/VirtualPlayer";
import { mongo } from "./MongoDB/index"
import { IVirtualPlayer, VirtualPlayer } from "../../Models/VirtualPlayer";

const COLLECTION: string = "VirtualPlayer"

export async function getAll(): Promise<Array<IVirtualPlayer>> {
    const data = await mongo(COLLECTION).find().toArray()

    return DocumentsToArray<IVirtualPlayer>(data, toVirtualPlayer)
}

export async function get(user_id: string): Promise<IVirtualPlayer> {
    const data = await mongo(COLLECTION).findOne({ user_id })

    return toVirtualPlayer(data)
}

export async function create(vp: ICreateVirtualPlayer): Promise<IVirtualPlayer> {

    const _id = `${uuid()}`
    const newVirtualPlayer = new VirtualPlayer(
        _id,
        vp.user_id,
        vp.personality,
        []
    )

    await mongo(COLLECTION).insertOne(newVirtualPlayer)
    const data = await mongo(COLLECTION).findOne({ _id })

    return toVirtualPlayer(data)
}

export async function update(vp: IUpdateVirtualPlayer): Promise<IVirtualPlayer> {

    await mongo(COLLECTION).findOneAndUpdate({ _id: vp._id }, { $set: vp })

    return toVirtualPlayer(await get(vp._id))
}

export function toVirtualPlayer(game: IVirtualPlayer): IVirtualPlayer {
    return new VirtualPlayer(
        game._id,
        game.user_id,
        game.personality,
        game.messages_used
    )
}