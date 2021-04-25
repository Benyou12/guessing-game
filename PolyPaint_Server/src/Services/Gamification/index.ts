import { IBadge, UserBadge } from "../../Models/Badge";
import * as UserDB from "../../Services/DataStore/User"
import { getCurrentTimestamp } from "../Time/GetTime";
import { Levels } from "./Levels"
import { EmitInRoom } from "../../Socket";
import UserController from "../../Socket/Controllers/user"

export default async function addBadge(badge: IBadge, game_id: string, user_id: string) {

    const user = await UserDB.get(user_id)
    const userBadge = new UserBadge(getCurrentTimestamp(), game_id, badge)

    user.gamification.badges.push(userBadge)
    user.gamification.points += badge.points

    console.log("LEVELS", Levels)
    console.log("USER POINTS", user.gamification)

    const newLevel = Levels.find((level) => level.min_points >= user.gamification.points)
    const levelChanged = newLevel.number !== user.gamification.level.number

    user.gamification.level = newLevel

    await UserDB.update(user)
    
    if (levelChanged) {
        EmitInRoom([user_id], UserController.gamification.level.response, newLevel)
    }

    EmitInRoom([user_id], UserController.gamification.badge.response, userBadge)
}