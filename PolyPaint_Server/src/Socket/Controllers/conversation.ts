import { SocketContext } from "../intefaces"
import { IGetConversation, IGetConversations, ICreateConversationRequest, IAddUserToConvoRequest, ISearchConversationsResponse, IDeleteConversationRequest, IDeleteConversationResponse, IUpdateConversation, IInviteUserRequest, IInviteUserResponse } from "../../Interfaces/Conversations"
import { Context } from "koa"
import { IConversation, CONVERSATION_UPDATE } from "../../Models/Conversation"
import * as ConversationAction from "../../Services/DataStore/Conversations"

export default {
	get: {
		/**
		 * Return all the conversation for one user
		 */
		all: {
			action: (ctx: Context, payload: IGetConversations) => ConversationAction.getAll(payload.uid),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'conversation.all',
				payload: (ctx: SocketContext<IConversation[]>) => ctx.body
			}
		},
		/**
		 * Return all the conversations to allow a local search
		 */
		search: {
			action: (ctx: Context, payload: void) => ConversationAction.search(),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'conversation.search',
				payload: (ctx: SocketContext<ISearchConversationsResponse[]>) => ctx.body
			}
		},
		/**
		 * Returns one conversation based on it's ID
		 */
		one: {
			action: (ctx: Context, payload: IGetConversation) => ConversationAction.get(payload.cid),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'conversation.one',
				payload: (ctx: SocketContext<IConversation>) => ctx.body
			}
		},
	},
	post: {
		// POST: routes
		create: {
			action: (ctx: Context, payload: ICreateConversationRequest) => ConversationAction.create(payload),
			rooms: (_uid: string, ctx: SocketContext<IConversation>) => ctx.body.uids,
			response: {
				route: 'conversation.new',
				payload: (ctx: SocketContext<IConversation>) => ({ ...ctx.body, updateAction: CONVERSATION_UPDATE.CREATED })
			}
		},
		invite: {
			action: (ctx: Context, payload: IInviteUserRequest) => ConversationAction.invite(ctx, payload),
			rooms: (_uid: string, ctx: SocketContext<IInviteUserResponse>) => ([ctx.body.uid]),
			response: {
				route: 'conversation.invite',
				payload: (ctx: SocketContext<IInviteUserResponse>) => ctx.body
			}
		},
	},
	patch: {
		addUser: {
			action: (ctx: Context, payload: IAddUserToConvoRequest) => ConversationAction.addUser(payload.cid, payload.uid),
			rooms: (_uid: string, ctx: SocketContext<IConversation>) => ctx.body.uids,
			response: {
				route: 'conversation.updated',
				payload: (ctx: SocketContext<IConversation>) => ({ ...ctx.body, updateAction: CONVERSATION_UPDATE.USER_ADDED })
			}
		},
		removeUser: {
			action: (ctx: Context, payload: IAddUserToConvoRequest) => ConversationAction.removeUser(ctx, payload.cid, payload.uid),
			rooms: (_uid: string, ctx: SocketContext<IConversation>) => ctx.body.uids,
			response: {
				route: 'conversation.updated',
				payload: (ctx: SocketContext<IConversation>) => ({ ...ctx.body, updateAction: CONVERSATION_UPDATE.USER_REMOVED })
			}
		},
		update: {
			action: (ctx: Context, payload: IUpdateConversation) => ConversationAction.update(payload),
			rooms: (_uid: string, ctx: SocketContext<IConversation>) => ctx.body.uids,
			response: {
				route: 'conversation.updated',
				payload: (ctx: SocketContext<IConversation>) => ({ ...ctx.body, updateAction: CONVERSATION_UPDATE.UPDATED })
			}
		},
	},
	delete: {
		one: {
			action: (ctx: Context, payload: IDeleteConversationRequest) => ConversationAction.destroy(ctx, payload.cid),
			response: {
				route: 'conversation.deleted',
				payload: (ctx: SocketContext<IDeleteConversationResponse>) => ({ ...ctx.body, updateAction: CONVERSATION_UPDATE.DELETED })
			}
		}
	}
}