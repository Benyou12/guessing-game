import { JoinRoom, Action } from "./intefaces"
import { request } from "./Utils/request"
import { Context } from "vm"
import * as AuthActions from "../Services/DataStore/Auth"
import * as GameActions from "../Services/DataStore/Game"
const EventEmitter = require("events")

class MyEmitter extends EventEmitter {}
const events = new MyEmitter()

let hbeat = new Map()
let connexions = new Map()
let roomsMap = new Map()

export const EmitInRoom = async function(rooms, response, payload) {
    let ctx: Context = { body: payload }
    const data = await response.payload(ctx)
    events.emit("socketResponse", rooms, response.route, data)
}

export default (io: SocketIO.Server) => {
    // New connection
    io.on("connect", function(client: SocketIO.Socket) {
        console.log("SOCKET CONNECTED :: ", client.id)

        client.on("join-room", async function(data: string) {
            console.log("SOCKET JOIN ROOM :: ", typeof data, data)

            const parsedData = typeof data != "object" ? JSON.parse(data) : data

            const { uid }: JoinRoom = parsedData
            client.join(uid)
            console.log("Socket Joined Room", uid)
            io.sockets.in(uid).emit("room-joined", { uid })

            io.sockets.in(uid).emit("ping1", uid)
            connexions.set(uid, client.id)
            roomsMap.set(client.id, uid)

            client.on("action", async function(action: string) {
                try {
                    console.log("Action", typeof action, action)
                    const parsedAction =
                        typeof action != "object" ? JSON.parse(action) : action
                    const response = await request(parsedAction, uid, io)

                    if (response && !['message.all', 'canvas.stroke.updated', 'game.all', 'game.active'].includes(response.route)) {
                        console.log("RESPONSE", response)
                    }
                    
                    if (response && response.payload) {
                        if (Array.isArray(response.rooms)) {
                            response.rooms.forEach(function(room: string) {
                                io.sockets
                                    .in(room)
                                    .emit(response.route, response.payload)
                            })
                        } else if (response && response.rooms == "all") {
                            io.sockets.emit(response.route, response.payload)
                        }
                    }
                } catch (err) {
                    console.log("A handled error occured.", err.message)
                }
            })

            client.on('pong', () => {
                console.log("SOCKET PING :: ", client.id, roomsMap.get(client.id))
            })

            client.on('error', (error) => {
                console.log("SOCKET ERROR :: ", client.id, roomsMap.get(client.id), error)
            })
        
            client.on('disconnecting', (reason) => {
                console.log("SOCKET DISCONNECTING :: ", client.id, roomsMap.get(client.id), reason)
            })

            client.on("disconnect", async function(reason) {
                console.log("SOCKET DISCONNECTED :: ", client.id, roomsMap.get(client.id), reason)

                const uid: string = roomsMap.get(client.id)
                console.log("The socket is the same: ", connexions.get(uid) === client.id)
                if (connexions.get(uid) === client.id) {
                    // await AuthActions.signOut(uid)
                }
                // await GameActions.cancelWithUser(uid)

                connexions.delete(uid)
                roomsMap.delete(client.id)
                hbeat.delete(client.id)
            })
        })
    })


    events.on("socketResponse", function(
        rooms: string[],
        route: string,
        data: any
    ) {
        if (!route || !['message.all', 'canvas.stroke.updated', 'game.all', 'game.active'].includes(route)) {
            console.log("RESPONSE2", { route, data })
        }

        if (Array.isArray(rooms)) {
            rooms.forEach(function(room: string) {
                io.sockets
                    .in(room)
                    .emit(route, data)
            })
        } else if (rooms == "all") {
            io.sockets.emit(route, data)
        }
        
    })
}
