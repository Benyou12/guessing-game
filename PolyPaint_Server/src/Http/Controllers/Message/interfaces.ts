import { IMessage } from "../../../Models/Message";

export interface IGetMessages {
    cid: string
}

export interface ICreateMessageRequest {
    cid: string,
    message: {
        text: string,
        user: {
            uid: string
        }
    }
}


export interface ICreateMessageResponse {
    cid: string,
    message: IMessage
}