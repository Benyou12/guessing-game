import * as Koa from 'koa';
import * as Router from 'koa-router';
import * as bodyParser from 'koa-body'
import modules from "./Http/Controllers/index";
import configs from "./Configs/index"
import socket from "./Socket"
import { UploadFile, ResizeImg } from "./Services/CloudStorage/FirebaseStorage"
import { InitDB } from "./Services/DataStore/MongoDB/index"
import * as UserActions from "./Services/DataStore/User"
import { IUser } from './Models/User';

const app = new Koa();
const router = new Router();

modules(app)

app.use(bodyParser({
    formidable:{},
    multipart: true,
    urlencoded: true
}))


var server = require('http').Server(app.callback());
const io = require('socket.io')(server, {
    pingInterval: 500,
    pingTimeout: 10000,
    upgradeTimeout: 30000
})

socket(io)

const passport = require('./Services/Auth/index').AuthInit(io)
const SendLoginResponse = require('./Services/Auth/index').SendLoginResponse
app.use(passport.initialize())

const LOGIN_SUCCESS = `Login successful. You can close this page. / Connexion réussie. Vous pouvez fermer cette page.
<script>window.close();</script>
`

router.get('/auth/facebook', function (ctx) {
    console.log("CTX QUERY", ctx.query)
    passport.authenticate('facebook', {
        //authType: 'reauthenticate',
        scope: [
            'public_profile',
            'email',
        ],
        session: false,
        callbackURL: `https://log3900.lbacreations.com/auth/facebook/callback`,
        state: ctx.query.login_id
    })(ctx)
})
  
router.get('/auth/facebook/callback', async function (ctx) {
    return new Promise((resolve, reject) => {
        passport.authenticate('facebook', {
            callbackURL: `https://log3900.lbacreations.com/auth/facebook/callback`,
            session: false
        }, function (err, profile) {
            if (err) {
                ctx.body = `Login error: ${err.message}`
                resolve()
                return
            }
            SendLoginResponse(ctx.query.state, profile, io)
            ctx.type = 'html'
            ctx.body = LOGIN_SUCCESS
            resolve()
        })(ctx)
    }) 
})

router.get('/auth/google',
    function (ctx) {
        passport.authenticate('google', {
            scope: ['profile', 'email'],
            session: false,
            callbackURL: `https://log3900.lbacreations.com/auth/google/callback`,
            state: ctx.query.login_id
        })(ctx)
    }
)
  
router.get('/auth/google/callback', async function (ctx) {
    return new Promise((resolve, reject) => {
        passport.authenticate('google', {
            callbackURL: `https://log3900.lbacreations.com/auth/google/callback`,
            session: false
        }, function (err, profile) {
            if (err) {
                ctx.body = `Login error: ${err.message}`
                resolve()
                return
            }
            SendLoginResponse(ctx.query.state, profile, io)
            ctx.type = 'html'
            ctx.body = LOGIN_SUCCESS
            resolve()
        })(ctx)
    }) 
})

router.get('/auth/github',
    function (ctx) {
        passport.authenticate('github', {
            scope: ['profile'],
            session: false,
            callbackURL: `https://log3900.lbacreations.com/auth/github/callback?login_id=${ctx.query.login_id}`
        })(ctx)
    }
)
  
router.get('/auth/github/callback', async function (ctx) {
    return new Promise((resolve, reject) => {
        passport.authenticate('github', {
            callbackURL: `https://log3900.lbacreations.com/auth/github/callback?login_id=${ctx.query.login_id}`,
            session: false
        }, function (err, profile) {
            console.log("GITHUB CALLBACK", err, profile)
            if (err) {
                ctx.body = `Login error: ${err.message}`
                resolve()
                return
            }
            SendLoginResponse(ctx.query.login_id, profile, io)
            ctx.type = 'html'
            ctx.body = LOGIN_SUCCESS
            resolve()
        })(ctx)
    }) 
})

router.post("/upload", async ctx => {
    const file = ctx.request.files.file;
    ctx.body = await UploadFile(file.type, file.name, file.path, ctx.request.body.folder)
});

router.get("/resize", async ctx => {
    const users: IUser[] = await UserActions.getAll()

    for (let user of users) {
        (function (uid) {
            ResizeImg(user.profileImgUrl).then((newImg) => {
                UserActions.update({
                    uid: uid,
                    profileImgUrl: newImg
                }).then((user) => {
                    console.log('NEW IMAGE', uid, user.profileImgUrl)
                })
            })
        })(user.uid)
    }

    ctx.body = 'Succeded!'
});

app.use(router.routes());

console.log("Starting Mongo DB and the server.")
InitDB().then(() => {
    server.listen(configs.port, () => {
        console.log(`Youpi! The server is running on at http://localhost:${configs.port}`)
    })
})

