import { Base, IBase } from "./Base";
import { IUser } from "./User";
import { IPlayerUser } from "./Player";

/**
 * VirtualPlayer Interfaces
 */
export enum PERSONALITY {
    PRETENTIOUS = "pretentious",
    CHOLERIC = "choleric",
    JOKER = "joker"
}

export interface IVirtualPlayer extends IBase {
    _id: string,
    user_id: string,
    personality: PERSONALITY,
    messages_used: string[],
    user?: IPlayerUser // NOTE: Pour les messages virtuels seulement. Ne pas inclure dans les clients.
}


/**
 * VirtualPlayer Class
 */
export class VirtualPlayer extends Base implements IVirtualPlayer, IBase {
    constructor(
        public _id: string,
        public user_id: string,
        public personality: PERSONALITY,
        public messages_used: string[]
    ){
        super()
    }
}
