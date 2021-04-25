import * as CanvasActions from "../../../Services/DataStore/Canvas"
import { Context } from "koa";
import { IUpdateStroke } from "./interfaces";
import { ICanvas } from "../../../Models/Canvas";


export async function get(ctx: Context, body: ICanvas) {
	const { _id } = body
	ctx.body = await CanvasActions.get(_id)
}

export async function create(ctx: Context, body: ICanvas) {
	ctx.body = await CanvasActions.create(body.uids)
}

export async function updateStrokes(ctx: Context, body: IUpdateStroke) {
	ctx.body = body
}