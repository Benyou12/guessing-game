import { SocketContext } from "../intefaces"
import { IGetConversation } from "../../Interfaces/Conversations"
import { Context } from "koa"
import * as UserActions from "../../Http/Controllers/User/controller"
import * as UserController from "../../Services/DataStore/User"
import { IUser } from "../../Models/User";
import { ICreateUser, IGetUser } from "../../Http/Controllers/User/interfaces"
import { ILevel } from "../../Models/Level";
import { IBadge, IUserBadge } from "../../Models/Badge";

export default {
	get: {
		all: {
			action: () => UserController.getAll(),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.all',
				payload: (ctx: SocketContext<IUser[]>) => ctx.body.map(user => {
					delete user.auth_history
					delete user.game_history
					return user
				})
			}
		},
		one: {
			action: (ctx: Context, payload: IGetUser) => UserActions.get(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.one',
				payload: (ctx: SocketContext<IUser>) => ctx.body
			}
		},
		online: {
			action: () => UserController.getOnline(),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.online',
				payload: (ctx: SocketContext<IUser[]>) => ctx.body
			}
		},
		inConversation: {
			action: (ctx: Context, payload: IGetConversation) => UserActions.getInConversation(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.conversation',
				payload: (ctx: SocketContext<IUser[]>) => ctx.body
			}
		},
	},
	post: {
		create: {
			action: (ctx: Context, payload: ICreateUser) => UserActions.create(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.created',
				payload: (ctx: SocketContext<IUser>) => ctx.body
			}
		},
	},
	gamification: {
		level: {
			action: (ctx: Context, payload: void) => () => {},
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.gamification.newLevel',
				payload: (ctx: SocketContext<ILevel>) => ctx.body
			}
		},
		badge: {
			action: (ctx: Context, payload: void) => () => {},
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'user.gamification.newBadge',
				payload: (ctx: SocketContext<IUserBadge>) => ctx.body
			}
		},
	}
}
