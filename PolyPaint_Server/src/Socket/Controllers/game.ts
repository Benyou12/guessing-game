import { SocketContext } from "../intefaces"
import { Context } from "koa"
import { IGame, GAME_UPDATE } from "../../Models/Game";
import { IGetGameRequest, ICreateGameRequest, IUpdateGameRequest, IDeleteGame, IAddVirtualPlayerRequest, ICancelGame } from "../../Interfaces/Game";
import { IAddPlayerRequest } from "../../Interfaces/Game";
import * as GameActions from "../../Services/DataStore/Game";
import { IAction } from "../../Models/Action";
import { IRound } from "../../Models/Round";
import { EmitInRoom } from "..";

const routes = {
	get: {
		all: {
			action: (ctx: Context, payload: void) => GameActions.getAll(),
			rooms: (uid: string) => [uid],
			response: {
				route: 'game.all',
				payload: (ctx: SocketContext<IGame[]>) => ctx.body
			}
		},
		active: {
			action: (ctx: Context, payload: void) => GameActions.getActive(),
			rooms: (uid: string) => [uid],
			response: {
				route: 'game.active',
				payload: (ctx: SocketContext<IGame[]>) => ctx.body
			}
		},
		one: {
			action: (ctx: Context, payload: IGetGameRequest) => GameActions.get(payload.game_id),
			rooms: (uid: string) => [uid],
			response: {
				route: 'game.one',
				payload: (ctx: SocketContext<IGame>) => removeCanvasStrokes(ctx.body)
			}
		},
	},
	post: {
		create: {
			action: (ctx: Context, payload: ICreateGameRequest) => GameActions.create(payload),
			rooms: (_uid: string, ctx: SocketContext<IGame>) => "all",
			response: {
				route: 'game.created',
				payload: (ctx: SocketContext<IGame>) => {
					updateActiveGamesList()
					return removeCanvasStrokes({
						...ctx.body,
						updateAction: GAME_UPDATE.CREATED
					})
				}
			}
		},
	},
	patch: {
		update: {
			action: (ctx: Context, payload: IUpdateGameRequest) => GameActions.update(payload),
			rooms: (_uid: string, ctx: SocketContext<IGame>) => GameActions.getUids(ctx.body),
			response: {
				route: 'game.updated',
				payload: (ctx: SocketContext<IGame>) => {
					updateActiveGamesList()
					return removeCanvasStrokes(ctx.body)
				}
			}
		},
		join: {
			action: (ctx: Context, payload: IAddPlayerRequest) => GameActions.addPlayer(payload._id, payload),
			rooms: (_uid: string, ctx: SocketContext<IGame>) => GameActions.getUids(ctx.body),
			response: {
				route: 'game.updated',
				payload: (ctx: SocketContext<IGame>) => {
					updateActiveGamesList()
					return removeCanvasStrokes({
						...ctx.body,
						updateAction: GAME_UPDATE.USER_JOIN
					})
				} 
			}
		},
		addVirtual: {
			action: (ctx: Context, payload: IAddVirtualPlayerRequest) => GameActions.addVirtualPlayer(payload),
			rooms: (_uid: string, ctx: SocketContext<IGame>) => GameActions.getUids(ctx.body),
			response: {
				route: 'game.updated',
				payload: (ctx: SocketContext<IGame>) => {
					updateActiveGamesList()
					return removeCanvasStrokes({
						...ctx.body,
						updateAction: GAME_UPDATE.VIRTUAL_JOIN
					})
				} 
			}
		},
		quit: {
			action: (ctx: Context, payload: IAddPlayerRequest) => GameActions.removePlayer(ctx, payload._id, payload),
			rooms: (_uid: string, ctx: SocketContext<IGame>) => GameActions.getUids(ctx.body),
			response: {
				route: 'game.updated',
				payload: (ctx: SocketContext<IGame>) => { 
					updateActiveGamesList()
					return removeCanvasStrokes({
						...ctx.body,
						updateAction: GAME_UPDATE.USER_QUIT
					})
				}
			}
		},
		action: {
			action: (ctx: Context, payload: IAction) => {
				const content = GameActions.action(payload)
				updateActiveGamesList()
				return content
			},
			rooms: (_uid: string, ctx: SocketContext<IGame>) => GameActions.getUids(ctx.body),
			response: {
				route: 'game.updated',
				payload: (ctx: SocketContext<IGame>) => removeCanvasStrokes({
					...ctx.body,
					updateAction: GAME_UPDATE.ACTION_FLOW
				})
			}
		},
		cancel: {
			action: () => {},
			rooms: (_uid: string) => [],
			response: {
				route: 'game.canceled',
				payload: (ctx: SocketContext<ICancelGame>) => {
					updateActiveGamesList()
					return { _id: null, desc: { fr: "Jeu annulé.", en: "Game canceled." } }
				} 
			}
		}
	},
	delete: {
		one: {
			action: (ctx: Context, payload: IDeleteGame) => GameActions.destroy(ctx, payload._id),
			rooms: (_uid: string, ctx: SocketContext<IDeleteGame>) => ctx.uids,
			response: {
				route: 'game.deleted',
				payload: (ctx: SocketContext<IDeleteGame>) => {
					updateActiveGamesList()
					return ctx.body
				}
			}
		}
	}
}

export default routes

/**
 * Helpers
 */

export function removeCanvasStrokes(game: IGame) {

	if (!game || !game.rounds || game.rounds.length === 0) {
		return game
	}

	const gameClone: IGame = JSON.parse(JSON.stringify(game))
	if (gameClone.rounds) {
		gameClone.rounds.map((round: IRound): IRound => {
			if (round.game_img && round.game_img.canvas) {
				round.game_img.canvas.strokes = []
			}

			return round
		})
	}

	return gameClone
}

export async function updateActiveGamesList() {
	EmitInRoom(
        "all",
        routes.get.active.response,
        await routes.get.active.action(null)
    )
}