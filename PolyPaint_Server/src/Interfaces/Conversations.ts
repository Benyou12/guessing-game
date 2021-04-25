import { IUser, User } from "../Models/User";
import { IMessage } from "../Models/Message";
import { IConversation } from "../Models/Conversation";

export interface IGetConversations {
    uid?: string
}

export interface ISearchConversationsResponse {
    cid: string,
    convName: string,
    timestamp: number,
    uids: string[],
    users: IUser[]
}

export interface IGetConversation {
    cid: string
}

export interface ICreateConversationRequest {
    uids: string[]
    convName: string
    messages?: IMessage[]
}

export interface IUpdateConversation {
    cid: string,
    convName: string
}

export interface IAddUserToConvoRequest {
    cid?: string,
    uid: string
}

export interface IDeleteConversationRequest {
    cid: string
}

export interface IDeleteConversationResponse {
    cid: string
}

export interface IInviteUserRequest {
    cid: string,
    uid: string // La personne à inviter
}

export interface IInviteUserResponse {
    cid: string,
    conversation: IConversation,
    messages: IMessage[],
    convName: string,
    uid: string, // La personne à inviter
    user: IUser // La personne qui a envoyé l'invitation
}