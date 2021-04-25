import * as MessagesActions from "../../../Services/DataStore/Message"
import { Context } from "koa";
import { ICreateMessageRequest, IGetMessages } from "./interfaces";


export async function getAll(ctx: Context, body: IGetMessages) {
	const { cid } = body
	ctx.body = await MessagesActions.getAll(cid)
}

export async function create(ctx: Context, body: ICreateMessageRequest) {
	ctx.body = await MessagesActions.create(body)
}