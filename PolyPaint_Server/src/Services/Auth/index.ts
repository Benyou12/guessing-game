import * as UserActions from "../DataStore/User"
import AuthController from "../../Socket/Controllers/auth"
import { EmitInRoom } from "../../Socket"
import { IUser } from "../../Models/User"
import { IOAuthResponse } from "../../Interfaces/Auth"
import { getCurrentTimestamp } from "../Time/GetTime"

const ERROR_EMAIL = `
Un courriel est requis. Veuillez utiliser un autre moyen de connexion.
Vous pouvez fermer le navigateur et retourner à l'application. 
- - -
A valid email address is required for the login. Please use another account.
You can close the browser and open the application.
`

export const AuthInit = function () {
    
    const passport = require('koa-passport')

    const FacebookStrategy = require('passport-facebook').Strategy
    passport.use(new FacebookStrategy({
        clientID: '1112413675619134',
        clientSecret: 'c40562ea002f39b103d9f9c10ae7b11a',
        profileFields: ['id', 'displayName', 'photos', 'email'],
    },
    function(token, tokenSecret, profile, done) {
        console.log("FACEBOOK AUTH", token, tokenSecret, profile, done)

        const [firstName, lastName] = profile.displayName.split(" ")

        done(null, {
            firstName,
            lastName,
            email: profile.emails && profile.emails.length  ? profile.emails[0].value : "",
            profileImgUrl: profile.photos[0].value
        })
    }
    ))

    const GoogleStrategy = require('passport-google-oauth20').Strategy
    passport.use(new GoogleStrategy({
        clientID: '972547814169-efkua5dpd4jj2pgacuiane9oeifk99md.apps.googleusercontent.com',
        clientSecret: '8s5TMhUMZacSk9tvzqMYU3FI',
        callbackURL: 'https://log3900.lbacreations.com/auth/google/callback'
    },
    function(accessToken, refreshToken, profile, done) {
        console.log("GOOGLE AUTH", accessToken, refreshToken, profile, done)

        done(null, {
            firstName: profile.name.givenName,
            lastName: profile.name.familyName,
            email: profile.emails && profile.emails.length  ? profile.emails[0].value : "",
            profileImgUrl: profile.photos[0].value
        })
    }
    ))

    const GithubStrategy = require('passport-github2').Strategy
    passport.use(new GithubStrategy({
        clientID: '00ccc384c2b805238c45',
        clientSecret: 'a4a76e5ee8b4896cc5f49b2292ba65adf6d30fbb',
        callbackURL: 'https://log3900.lbacreations.com/auth/github/callback'
    },
    function(accessToken, refreshToken, profile, done) {
        console.log("GITHUB AUTH", accessToken, refreshToken, profile, done)

        let firstName = "", lastName = ""

        if (profile && profile.displayName) {
            [firstName, lastName] = profile.displayName.split(" ")
        }

        done(null, {
            username: profile && profile.username,
            firstName,
            lastName,
            email: profile.emails && profile.emails.length ? profile.emails[0].value : "",
            profileImgUrl: profile.photos[0].value
        })
    }
    ))

    return passport
}

export const SendLoginResponse = async function (login_id, profile, io: SocketIO.Server) {
    console.log("SEND LOGIN RESPONSE", login_id, profile)

    let user: IUser = null

    if (profile.email) {
        user = await UserActions.getByEmail(profile.email)
        console.log("USER FROM FIND BY EMAIL", user)
    }

    let data: IOAuthResponse = {
        login_id,
        isLogin: false,
        user: {
            uid: undefined,
            firstName: profile.firstName,
            lastName: profile.lastName,
            email: profile.email || "",
            username: profile.username,
            profileImgUrl: profile.profileImgUrl
        }
    }

    if (user) {
        console.log("USER EXIST", user)

        await UserActions.createAuthHistory(user.uid, {
            isLogin: true,
            timestamp: getCurrentTimestamp()
        })
        
        data = {
            login_id,
            isLogin: true,
            user: user
        }   
    }

    console.log("MESSAGE TO EMIT", data)

    io.emit(AuthController.post.oaut.response.route, data)
}


