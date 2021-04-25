import { IUser } from "../Models/User";

export interface IOAuthResponse {
    isLogin: boolean,
    login_id: string,
    user: IUser
}