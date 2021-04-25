import * as UserActions from "../../../Services/DataStore/User"
import * as AuthActions from "../../../Services/DataStore/Auth"
import * as ConversationActions from "../../../Services/DataStore/Conversations"
import { User, IUser } from "../../../Models/User"
import { Context } from "koa";
import { ICreateUser, IGetUser, IUpdateGamification, ICreateGameStatHistory, ICreateAuthStats, IUpdateUser } from "./interfaces";
import { IGetConversation } from "../../../Interfaces/Conversations";
import { IUserGameStatsHistory } from "../../../Models/UserGameStatsHistory";

const GENERAL_CID = "general"

/**
 * 
 * @param ctx 
 * @returns User
 */
export async function create(ctx: Context, body: ICreateUser) {
	try {

		const { email, password, firstName, lastName, username, profileImgUrl } = body

		if (!firstName || firstName.length == 0) return ctx.throw(403, "No first name provided.")
		else if (!email || email.length == 0) return ctx.throw(403, "No email provided.")
		else if (!password || password.length == 0) return ctx.throw(403, "No password provided.")
		else if (!lastName || lastName.length == 0) return ctx.throw(403, "No last name provided.")
		else if (!username || username.length == 0) return ctx.throw(403, "No username provided.")

		if (await UserActions.usernameExist(username)) return ctx.throw(409, "The username must be unique.")

		const authResult: { user: IUser } = await AuthActions.signUp(email, password)
		await UserActions.create({ uid: authResult.user.uid, email, firstName, lastName, username, profileImgUrl })
		const user: IUser = await UserActions.get(authResult.user.uid)
		await ConversationActions.addUser(GENERAL_CID, authResult.user.uid)

		ctx.body = user
		
	} catch(err) {
		console.error(err)
		if (err.message) return ctx.throw(403, err.message)
		return ctx.throw(401, 'Error creating the account')
	}	
}

export async function get(ctx: Context, body: IGetUser) {
	const { uid } = body

	ctx.body = await UserActions.get(uid)
}

export async function getInConversation(ctx: Context, body: IGetConversation) {
	const { cid } = body
	
	ctx.body = await UserActions.getInConversation(cid)
}

export async function update(ctx: Context, body: IUpdateUser) {
	const user: IUpdateUser = body

	ctx.body = await UserActions.update(user)
}

export async function gamification(ctx: Context, body: IUpdateGamification) {
	const { uid, badge, level }: IUpdateGamification = body

	ctx.body = await UserActions.gamification(uid, badge, level)
}



