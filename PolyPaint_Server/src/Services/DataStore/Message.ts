import firebase from "./Firebase/Firebase"
import { DocumentsToArray } from "./MongoDB/utils"
import * as UserActions from "./User"
import * as uuid from "uuid/v4"
import { IMessage, Message } from "../../Models/Message";
import { ICreateMessageRequest, ICreateMessageResponse } from "../../Http/Controllers/Message/interfaces"
import { getCurrentTimestamp } from "../Time/GetTime";
import * as ConversationDB from "./Conversations"

import { mongo } from "./MongoDB/index"
import { IUser } from "../../Models/User";
import * as GameDB from "./Game";
import GameFlow from "../GameFlow";
const COLLECTION = "Conversation";

export async function getAll(cid: string): Promise<Array<IMessage>> {
    const convervation = await mongo(COLLECTION).aggregate([
        { $match : { cid } },
        { "$unwind" : "$messages"} , 
        { "$sort" : { "messages.timestamp" : 1 }}, 
        { "$group" : { "messages" : { "$push" : "$messages"} , "_id" : null}} ,
        { "$project" : { "_id" : 0 , "messages" : 1 }}
    ])

    const messages = await convervation.toArray()

    if (!messages || messages.length === 0) return []

    return DocumentsToArray<IMessage>(messages[0].messages, toMessage)
}

export async function create(request: ICreateMessageRequest, user: IUser = null): Promise<ICreateMessageResponse> {

    if (!request.cid) {
        throw Error("No conversation ID provided.")
    }

    user = user ? user: await UserActions.getBasic(request.message.user.uid)

    
    GameDB.getByConversation(request.cid).then((game) => {
        if (game) {
            const gf = new GameFlow()
            gf.setGame(game)
            gf.parseMessage(user.uid, request.message.text)
        }
    })

    const mid = uuid()
    const newMessage = new Message(
        mid,
        request.message.text,
        getCurrentTimestamp(),
        user
    )

    await mongo(COLLECTION).findOneAndUpdate({ cid: request.cid }, { $push: { messages: newMessage } })

    return {
        cid: request.cid,
        message: Object.assign({}, {...newMessage, user: Object.assign({}, newMessage.user) })
    }
}

export function toMessage(message: IMessage | firebase.firestore.DocumentData): IMessage {
    return new Message(
        message.mid,
        message.text,
        message.timestamp,
        message.user
    )
}