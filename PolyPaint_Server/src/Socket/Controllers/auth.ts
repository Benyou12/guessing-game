import { SocketContext } from "../intefaces"
import { Context } from "koa"
import * as AuthActions from "../../Http/Controllers/Auth/controller"
import { IUser } from "../../Models/User";
import { ICreateUser } from "../../Http/Controllers/User/interfaces"
import { ILogin, ILogout } from "../../Http/Controllers/Auth/interfaces";
import { IOAuthResponse } from "../../Interfaces/Auth";

export default {
	post: {
		login: {
			action: (ctx: Context, payload: ILogin) => AuthActions.login(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'auth.login',
				payload: (ctx: SocketContext<IUser>) => ctx.body
			}
		},
		logout: {
			action: (ctx: Context, payload: ILogout) => AuthActions.logout(ctx, payload),
			rooms: (uid: string) => ([uid]),
			response: {
				route: 'auth.logout',
				payload: (ctx: SocketContext<IUser>) => ctx.body
			}
		},
		oaut: {
			action: () => {}, // NE PAS APPELER CETTE FONCTION AVEC LE SOCKET
			rooms: () => ([]),
			response: {
				route: 'auth.oauth', // Réponse à écouter lorsque le login est fini
				payload: (ctx: SocketContext<IOAuthResponse>) => ctx.body
			}
		},
	}
}