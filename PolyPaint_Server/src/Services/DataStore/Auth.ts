import firebase from "./Firebase/Firebase"
import * as UserActions from "../../Services/DataStore/User"
import * as GameActions from "../../Services/DataStore/Game"
import { getCurrentTimestamp } from "../Time/GetTime"
import { User, IUser } from "../../Models/User"
import { EmitInRoom } from "../../Socket"
import AuthController from "../../Socket/Controllers/auth"

export function signUp(email: string, password: string): Promise<any> {
    return firebase.auth().createUserWithEmailAndPassword(email, password)
}

export async function signIn(email: string, password: string): Promise<any> {

    var result = await firebase
            .auth()
            .signInWithEmailAndPassword(email, password)

    const uid: string = result.user.uid
    const user: IUser = await UserActions.get(uid)

    console.log("USER IS CONNECTED", user.isConnected)

    if (user.isConnected) {
        EmitInRoom([uid], AuthController.post.logout.response, { uid })
        await signOut(uid)
    }

    await UserActions.update({ uid: uid, isConnected: true })

    await UserActions.createAuthHistory(uid, {
        isLogin: true,
        timestamp: getCurrentTimestamp()
    })

    return UserActions.get(uid)
}

export async function signOut(uid: string): Promise<void> {
    const user: IUser = await UserActions.get(uid)

    if (user.isConnected) {
        await UserActions.createAuthHistory(uid, {
            isLogin: false,
            timestamp: getCurrentTimestamp()
        })

        await UserActions.update({ uid: uid, isConnected: false })

        await GameActions.cancelWithUser(uid)
    }
}