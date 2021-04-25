import { UserGameStats, IUserGameStats } from "./UserGameStats";
import { IUserGameStatsHistory, UserGameStatsHistory } from "./UserGameStatsHistory";
import { IUserAuthStats, UserAuthStats } from "./UserAuthStats";
import { IUserGamification, UserGamification } from "./UserGamification";
import { IBase, Base } from "./Base";

export interface IUser extends IBase {
    uid: string;
    firstName: string;
    lastName: string;
    email: string;
    username: string;
    profileImgUrl: string;
    game_stats?: IUserGameStats;
    game_history?: IUserGameStatsHistory[],
    auth_history?: IUserAuthStats[],
    gamification?: IUserGamification,
    isConnected?: boolean
}

export class BasicUser implements IUser {
    constructor(
        public uid: string,
        public firstName: string,
        public lastName: string,
        public email: string,
        public username: string,
        public profileImgUrl: string,
    ){
    }
}

export class User extends BasicUser implements IUser {
    constructor(
        public uid: string,
        public firstName: string,
        public lastName: string,
        public email: string,
        public username: string,
        public profileImgUrl: string,
        public game_stats: IUserGameStats = new UserGameStats(),
        public game_history: IUserGameStatsHistory[] = [],
        public auth_history: IUserAuthStats[] = [],
        public gamification: IUserGamification = new UserGamification(),
        public isConnected: boolean = false
    ){
        super(uid, firstName, lastName, email, username, profileImgUrl)
    }
}