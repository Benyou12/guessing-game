import { SocketContext } from "../intefaces"
import { Context } from "koa"
import * as GameImageController from "../../Services/DataStore/GameImage"
import { IGameImage } from "../../Models/GameImage";
import { IGetGameImageRequest, ICreateGameImageRequest, IUpdateGameImageRequest } from "../../Interfaces/GameImage";

export default {
	get: {
		all: {
			action: (ctx: Context, payload: void) => GameImageController.getAll(),
			rooms: (uid: string) => [uid],
			response: {
				route: 'gameImage.all',
				payload: (ctx: SocketContext<IGameImage[]>) => ctx.body
			}
		},
		random: {
			action: (ctx: Context, payload: void) => GameImageController.getRandom([]),
			rooms: (uid: string) => [uid],
			response: {
				route: 'gameImage.random',
				payload: (ctx: SocketContext<IGameImage[]>) => ctx.body
			}
		},
		one: {
			action: (ctx: Context, payload: IGetGameImageRequest) => GameImageController.get(payload._id),
			rooms: (uid: string) => [uid],
			response: {
				route: 'gameImage.one',
				payload: (ctx: SocketContext<IGameImage>) => ctx.body
			}
		},
	},
	post: {
		create: {
			action: (ctx: Context, payload: ICreateGameImageRequest) => GameImageController.create(payload),
			rooms: (uid: string) => [uid],
			response: {
				route: 'gameImage.created',
				payload: (ctx: SocketContext<IGameImage>) => ctx.body
			}
		},
	},
	patch: {
		update: {
			action: (ctx: Context, payload: IUpdateGameImageRequest) => GameImageController.update(payload),
			rooms: (uid: string) => [uid],
			response: {
				route: 'gameImage.updated',
				payload: (ctx: SocketContext<IGameImage>) => ctx.body
			}
		}
	}
}