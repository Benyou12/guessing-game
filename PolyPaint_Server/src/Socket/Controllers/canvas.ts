import { SocketContext } from "../intefaces"
import { Context } from "koa"
import * as CanvasController from "../../Http/Controllers/Canvas/controller"
import * as CanvasAction from "../../Services/DataStore/Canvas"
import { IUpdateStroke } from "../../Http/Controllers/Canvas/interfaces"
import { IStroke, ICanvas } from "../../Models/Canvas"


export default {
	get: {
		one: {
			action: (ctx: Context, payload: ICanvas) => CanvasController.get(ctx, payload),
			rooms: (uid: string) => [uid],
			response: {
				route: 'canvas.one',
				payload: (ctx: SocketContext<ICanvas>) => ctx.body
			}
		},
	},
	post: {
		create: {
			action: (ctx: Context, payload: ICanvas) => CanvasController.create(ctx, payload),
			rooms: (_uid: string, ctx: SocketContext<ICanvas>) => ctx.body.uids,
			response: {
				route: 'canvas.created',
				payload: (ctx: SocketContext<ICanvas>) => ctx.body
			}
		},
	},
	patch: {
		stroke: {
			action: (ctx: Context, payload: IUpdateStroke) => CanvasController.updateStrokes(ctx, payload),
			rooms: async (_uid: string, ctx: SocketContext<IStroke>) => (await getCanvasPlayer(ctx.body.canvas_id)).filter(uid => uid != _uid),
			response: {
				route: 'canvas.stroke.updated',
				payload: (ctx: SocketContext<IStroke>) => ctx.body
			}
		},
	}
}

let canvasCache: Map<string, string[]> = new Map()

async function getCanvasPlayer(_id: string): Promise<string[]> {

	if (canvasCache.has(_id)) {
		return canvasCache.get(_id)
	}

	canvasCache.set(_id, (await CanvasAction.get(_id)).uids)

	return canvasCache.get(_id)
}


