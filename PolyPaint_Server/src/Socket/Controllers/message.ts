import { SocketContext } from "../intefaces"
import { IGetMessages, ICreateMessageRequest, ICreateMessageResponse } from "../../Http/Controllers/Message/interfaces"
import { IMessage } from "../../Models/Message"
import { Context } from "koa"
import * as MessagesActions from "../../Http/Controllers/Message/controller"
import * as ConversationDB from "../../Services/DataStore/Conversations"

export default {
	get: {
		// GET: routes
		all: {
			action: (ctx: Context, payload: IGetMessages) => MessagesActions.getAll(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'message.all',
				payload: (ctx: SocketContext<IMessage[]>) => ctx.body
			}
		}
	},
	post: {
		// POST: routes
		create: {
			action: (ctx: Context, payload: ICreateMessageRequest) => MessagesActions.create(ctx, payload),
			rooms: async (_uid: string, ctx: SocketContext<ICreateMessageRequest>) => {
				console.log("CTX 2", ctx.body.cid)
				return (await ConversationDB.get(ctx.body.cid)).uids
			},
			response: {
				route: 'message.new',
				payload: (ctx: SocketContext<ICreateMessageResponse>) => ctx.body
			}
		}
	}
}