import firebase from "./Firebase/Firebase"
import { IConversation, Conversation } from "../../Models/Conversation";
import { DocumentsToArray } from "./MongoDB/utils"
import * as UserActions from "./User"
import * as uuid from "uuid/v4"
import { IUser } from "../../Models/User"
import { ICreateConversationRequest, ISearchConversationsResponse, IDeleteConversationResponse, IUpdateConversation, IInviteUserRequest, IInviteUserResponse } from "../../Interfaces/Conversations";
import { getCurrentTimestamp } from "../Time/GetTime";
import { EmitInRoom } from "../../Socket/index"
import ConversationController from "../../Socket/Controllers/conversation"
import * as MessagesActions from "../DataStore/Message"
import { Context } from "koa";

import { mongo } from "./MongoDB/index"
import user from "../../Socket/Controllers/user";

const COLLECTION = "Conversation";

export async function getAllServer(): Promise<Array<IConversation>> {
    const data = await mongo(COLLECTION).find({ $query: {}, $orderby: { timestampUpdated : -1 } }).toArray()

    return DocumentsToArray<IConversation>(data, toConversationReduced)
}

export async function getAll(uid: string): Promise<Array<IConversation>> {
    const data = await mongo(COLLECTION).find({ $query: { uids: { $elemMatch: { $eq: uid } } }, $orderby: { timestampUpdated : -1 } }).toArray()

    return DocumentsToArray<IConversation>(data, toConversation)
}

export async function search(): Promise<Array<ISearchConversationsResponse>> {
    const data = await mongo(COLLECTION).find({ $query: {}, $orderby: { timestampUpdated : -1 } }).toArray()
    const conversations: IConversation[] = DocumentsToArray<IConversation>(data, toConversation)

    return conversations
}

export async function get(cid: string): Promise<IConversation> {
    const data = await mongo(COLLECTION).findOne({ cid })

    return data ? toConversation(data) : null
}

export async function create(conversation: ICreateConversationRequest): Promise<IConversation> {

    if (!conversation.uids.length) {
        throw Error("No user in conversation.")
    }

    const users: IUser[] = []
    for (const uid of conversation.uids) {
        users.push(await UserActions.getBasic(uid))
    }

    const cid: string = uuid()
    const newConversation = new Conversation(
        cid,
        conversation.convName,
        getCurrentTimestamp(),
        getCurrentTimestamp(),
        conversation.uids,
        conversation.messages || [],
        users
    )

    await mongo(COLLECTION).insertOne(newConversation)
    const data = await mongo(COLLECTION).findOne({ cid: cid })

    return toConversation(data)
}

export async function update(payload: IUpdateConversation): Promise<IConversation> {

    await mongo(COLLECTION).findOneAndUpdate({ cid: payload.cid }, { $set: {
        ...payload,
        updatedTimestamp: getCurrentTimestamp()
    } })

    return toConversation(await get(payload.cid))
}

export async function addUser(cid: string, uid: string): Promise<IConversation> {
    const conversation: IConversation = await get(cid);

    if (!conversation)
        throw Error(JSON.stringify({ en: "This conversation id does not exist.", fr: "Le ID de cette conversation n'existe pas." }))

    if (conversation.uids.includes(uid)) {
        throw Error(JSON.stringify({ en: "The user is already in the conversation.", fr: "L'utilisateur est déjà dans la conversation." }))
    }

    const user: IUser = await UserActions.getBasic(uid)

    await mongo(COLLECTION).findOneAndUpdate(
        { cid }, 
        { $push: { 
            uids: uid,
            users: user
        }})

    return toConversation(await get(cid))
}

export async function removeUser(ctx: Context, cid: string, uid: string): Promise<IConversation | { cid: string }> {
    const conversation = await get(cid)

    if (!conversation)
        return

    ctx.uids = conversation.uids

    if (conversation.uids.length > 1) {
        await mongo(COLLECTION).findOneAndUpdate(
            { cid }, 
            { $pull: { 
                uids: uid,
                users: { uid }
            }})

        return toConversation(await get(cid))
    } else {
        await destroy(ctx, cid)

        EmitInRoom(
            conversation.uids,
            ConversationController.delete.one.response,
            { cid }
        )

        return { cid }
    }
}

export async function destroy(ctx?: Context, cid?: string): Promise<{ cid: string }> {
    const conversation = await get(cid)

    if (ctx && conversation) {
        ctx.uids = conversation.uids;
    }

    if (conversation) await mongo(COLLECTION).deleteOne({ cid })

    return { cid }
}

export async function invite(ctx: Context, payload: IInviteUserRequest): Promise<IInviteUserResponse> {

    const conversation = await get(payload.cid)
    const messages = await MessagesActions.getAll(conversation.cid)

    return {
        ...payload,
        conversation,
        messages,
        convName: conversation.convName,
        user: await UserActions.getBasic(ctx.uid)
    }
}

export function toConversation(conversation: IConversation | any): IConversation {
    return new Conversation(
        conversation.cid,
        conversation.convName,
        conversation.timestamp,
        conversation.updatedTimestamp,
        conversation.uids,
        conversation.messages,
        conversation.users
    )
}

export function toConversationReduced(conversation: IConversation | any): IConversation {
    return new Conversation(
        conversation.cid,
        conversation.convName,
        conversation.timestamp,
        conversation.updatedTimestamp,
        conversation.uids,
        [],
        []
    )
}