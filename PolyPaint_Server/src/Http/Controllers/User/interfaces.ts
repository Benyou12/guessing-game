import { IUser } from "../../../Models/User";
import { IUserGameStatsHistory } from "../../../Models/UserGameStatsHistory";
import { IUserAuthStats } from "../../../Models/UserAuthStats";
import { IUserGameStats } from "../../../Models/UserGameStats";
import { IUserGamification } from "../../../Models/UserGamification";

export interface ICreateUser extends IUser {
    password: string
}

export interface IGetUser {
    uid: string
}

export interface IUpdateUser {
    uid: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    username?: string;
    profileImgUrl?: string;
    game_stats?: IUserGameStats;
    game_history?: IUserGameStatsHistory[],
    auth_history?: IUserAuthStats[],
    gamification?: IUserGamification,
    isConnected?: boolean
}

export interface IUpdateGamification {
    uid: string,
    badge?: string,
    level?: string
}

export interface ICreateGameStatHistory extends IUserGameStatsHistory {
    uid: string
}

export interface ICreateAuthStats extends IUserAuthStats {
    uid: string
}