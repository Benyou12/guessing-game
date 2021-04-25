import { Base, IBase } from "./Base";

export interface IUserAuthStats extends IBase {
    isLogin: boolean
    timestamp: number
}

export class UserAuthStats extends Base implements IUserAuthStats, IBase {
    constructor(
        public isLogin: boolean,
        public timestamp: number
    ) {
        super()
    }
}