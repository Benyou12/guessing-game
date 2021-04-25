import firebase from "./Firebase/Firebase"
import { IUser, User, BasicUser } from "../../Models/User";
import { DocumentsToArray } from "./MongoDB/utils";
import { IUserGameStatsHistory } from "../../Models/UserGameStatsHistory";
import { IUserAuthStats, UserAuthStats } from "../../Models/UserAuthStats";
import { IUpdateUser } from "../../Http/Controllers/User/interfaces";
import * as ConversationDB from "./Conversations"
import { ResizeImg } from "../CloudStorage/FirebaseStorage"

import { mongo } from "./MongoDB/index"
import { UserGamification } from "../../Models/UserGamification";

const defaultProfilePicture = "https://firebasestorage.googleapis.com/v0/b/polychat-9c90e.appspot.com/o/default_profile_picture.png?alt=media"
const COLLECTION = "User";



export async function getAll(): Promise<IUser[]> {
const data: IUser[] = await mongo(COLLECTION).find({ uid: { $not: /^virtual.*/ } }).sort({ 'gamification.points': -1 }).toArray()

    return DocumentsToArray(data, toUser)
}

export async function create(user: any): Promise<IUser> {

    const newUser = new User(
        user.uid,
        user.firstName,
        user.lastName,
        user.email,
        user.username,
        user.profileImgUrl,
        user.game_stats,
        user.game_history,
        user.auth_history,
        user.gamification,
        user.isConnected
    )

    await mongo(COLLECTION).insertOne(newUser)

    console.log("USER PROFILE IMG", newUser.profileImgUrl)
    
    const newImg = await ResizeImg(newUser.profileImgUrl)
    await update({
        uid: newUser.uid,
        profileImgUrl: newImg
    })

    const data = await mongo(COLLECTION).findOne({ uid: user.uid })

    return toUser(data)
}

export async function get(uid: string): Promise<IUser> {
    const data = await mongo(COLLECTION).findOne({ uid })

    return toUser(data)
}

export async function getBasic(uid: string): Promise<IUser> {
    const data = await mongo(COLLECTION).findOne({ uid })

    return toBasicUser(data)
}

export async function getByEmail(email: string): Promise<IUser> {
    const data = await mongo(COLLECTION).findOne({ email })

    return data ? toUser(data) : null
}

export async function getInConversation(cid: string): Promise<Array<IUser>> {
    const convervation = await ConversationDB.get(cid)

    return DocumentsToArray(convervation.users, toBasicUser)
}

export async function getOnline(): Promise<IUser[]> {
    const data: IUser[] = await mongo(COLLECTION).find({ isConnected: true }).toArray()

    return DocumentsToArray(data, toBasicUser)
}

export async function usernameExist(username: string): Promise<boolean> {
    const data = await mongo(COLLECTION).findOne({ username: username })

    return data && data.uid
}

export async function update(user: IUpdateUser): Promise<IUser> {

    await mongo(COLLECTION).findOneAndUpdate({ uid: user.uid }, { $set: user })

    return toUser(await get(user.uid))
}

export async function gamification(uid: string, badge?: string, level?: string): Promise<IUser> {
    return get(uid)
}


export async function createGameStatHistory(uid: string, stat: IUserGameStatsHistory): Promise<IUser> {

    let payload = {
        game_id: stat.game_id,
        timestamp: stat.timestamp,
        names: stat.names,
        myTeamResult: stat.myTeamResult,
        otherTeamResult: stat.otherTeamResult,
        name: stat.name
    }

    await mongo(COLLECTION).findOneAndUpdate({ uid }, { $push: { game_history: payload } })

    return toUser(await get(uid))
}

export async function createAuthHistory(uid: string, stat: IUserAuthStats): Promise<IUser> {

    let payload = {
        isLogin: stat.isLogin,
        timestamp: stat.timestamp
    }

    await mongo(COLLECTION).findOneAndUpdate({ uid }, { $push: { auth_history: payload } })

    return toUser(await get(uid))
}

export function toUser(user: IUser | any): IUser {
    return new User(
        user.uid,
        user.firstName,
        user.lastName,
        user.email,
        user.username,
        user.profileImgUrl.length ? user.profileImgUrl : defaultProfilePicture,
        !user.game_stats.total_games_played ? { ...user.game_stats, total_games_played: 0 } : user.game_stats,
        user.game_history,
        user.auth_history,
        !user.gamification || user.gamification.level === "" ? new UserGamification(): user.gamification,
        !!user.isConnected
    )
}

export function toBasicUser(user: IUser | any): IUser {
    return new BasicUser(
        user.uid,
        user.firstName,
        user.lastName,
        user.email,
        user.username,
        user.profileImgUrl.length ? user.profileImgUrl : defaultProfilePicture
    )
}