import { Context } from "koa";
import { getCurrentTimestamp } from "../../../Services/Time/GetTime"


export async function getTimestamp(ctx: Context) {
	ctx.body = {
		timestamp: getCurrentTimestamp()
	}	
}