import { IMessage } from "./Message";
import { IUser } from "./User";
import { IBase, Base } from "./Base";

export enum CONVERSATION_UPDATE {
    DEFAULT = 0,
    CREATED = 1,
    USER_ADDED = 2,
    USER_REMOVED = 3,
    UPDATED = 4,
    DELETED = 5
}

export interface IConversation {
    cid: string
    convName: string
    timestamp: number
    updatedTimestamp: number
    uids: string[]
    messages: IMessage[],
    users: IUser[],
    updateAction: CONVERSATION_UPDATE
}

export class Conversation implements IConversation {

    constructor(
        public cid: string,
        public convName: string,
        public timestamp: number,
        public updatedTimestamp: number,
        public uids: string[],
        public messages: IMessage[] = [],
        public users: IUser[] = [],
        public updateAction: CONVERSATION_UPDATE = CONVERSATION_UPDATE.DEFAULT
    ) {}

}