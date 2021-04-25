import { Base, IBase } from "./Base";
import { IUser, User } from "./User";

/**
 * Player Interfaces
 */
export enum PLAYER_ROLE {
    DRAW = "DRAWER",
    GUESS = "GUESSER",
    NONE = "NONE"
}

export interface IPlayer extends IBase {
    _id: string,
    user_id: string,
    user: IPlayerUser,
    role: PLAYER_ROLE
}

export interface IPlayerUser extends IBase {
    uid: string,
    username: string,
    profileImgUrl: string
}

/**
 * Player Class
 */
export class Player extends Base implements IPlayer, IBase {
    constructor(
        public _id: string,
        public user_id: string,
        public user: IPlayerUser,
        public role: PLAYER_ROLE = PLAYER_ROLE.NONE
    ){
        super()
    }
}

export class PlayerUser extends Base implements IPlayerUser, IBase {
    constructor(
        public uid: string,
        public username: string,
        public profileImgUrl: string
    ){
        super()
    }
}