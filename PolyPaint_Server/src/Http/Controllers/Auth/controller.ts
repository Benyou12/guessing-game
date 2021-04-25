import * as AuthActions from "../../../Services/DataStore/Auth"

import { IUser } from "../../../Models/User"
import { ILogin, ILogout } from "./interfaces"
import { Context } from "vm";
import { getCurrentTimestamp } from "../../../Services/Time/GetTime";

/**
 * @param ctx Context
 * @param body ILogin
 * @returns IUser
 */
export async function login(ctx: Context, body: ILogin) {
	try {
		const { email, password }: ILogin = body

		ctx.body = await AuthActions.signIn(email, password)

	} catch(err) {
		console.error(err)
		if (err.message) return ctx.throw(403, err.message)
		return ctx.throw(401, 'Error creating the account')
	}	
}

/**
 * @param ctx Context
 * @param body ILogout
 * @returns ILogout
 */
export async function logout(ctx: Context, body: ILogout) {
	const { uid }: ILogout = body
	
	await AuthActions.signOut(uid);

	ctx.body = { uid }
}
