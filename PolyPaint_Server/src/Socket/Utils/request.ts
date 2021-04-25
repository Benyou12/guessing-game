
import { Action, SocketRoute, SocketError, SocketContext } from "../intefaces"

export const request = async function({ route, payload }: Action, uid: string, io: SocketIO.Server) {
	
	try {
		const routeParts: string[] = route.split(".")

		const module: any = require(`../Controllers/${routeParts[0]}`).default
		const defs: SocketRoute = module[routeParts[1]][routeParts[2]]

		//console.log("DEFS", defs)

		let error: SocketError = null

		let ctx: SocketContext<object> = {
			uid,
			request: {
				body: payload
			},
			body: null,
			uids: [],
			throw: (code: number, message: string) => { error = { code, message } }
		}

		const body = await defs.action(ctx, payload)

		if (typeof body !== "undefined")
			ctx.body = body

		if (error) {
			console.error(error)
			io.sockets.in(uid).emit('error', { code: error.code, message: error.message });
			return;
		}

		const rooms: string[] = ctx.uids.length > 0 ? ctx.uids : await defs.rooms(uid, ctx)
		const content: object = await defs.response.payload(ctx) 

		return {
			route: defs.response.route,
			rooms: rooms,
			payload: content,
		}
	} catch (error) {
		console.error("Request Not Treated: ", error)
		io.sockets.in(uid).emit('error', { code: error.code, message: error.message });
	}

}