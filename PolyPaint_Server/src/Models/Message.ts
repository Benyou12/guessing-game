import { IUser } from "./User";
import { IBase } from "./Base";

export interface IMessage extends IBase {
    mid: string;
    text: string;
    timestamp: number;
    user: IUser
}

export class Message  implements IMessage, IBase {

    constructor(
        public mid: string,
        public text: string,
        public timestamp: number,
        public user: IUser
    ) {}

    public toObject(): object {
        return Object.assign({}, this, {
            user: this.user.toObject()
        })
    }
}