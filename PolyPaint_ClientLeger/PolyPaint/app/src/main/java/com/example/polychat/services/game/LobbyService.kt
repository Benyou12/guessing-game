package com.example.polychat.services.game

import com.example.polychat.models.socket.AddPlayerRequest
import com.example.polychat.models.socket.GameEvent
import com.example.polychat.models.socket.SocketAction
import com.example.polychat.models.socket.Uid
import com.example.polychat.services.serverServices.SocketSingleton

class LobbyService {

    companion object{
        val action = "action"

        fun createNewGame(uid: String) {
            val route = GameEvent.CREATE_GAME.event
            val uId = Uid(uid)
            val socketAction = SocketAction(route, uId)
            SocketSingleton.emit(action, socketAction)

        }

        fun getActiveGames(){
            val route = GameEvent.GET_ACTIVE_GAME.event
            val socketAction = SocketAction(route, Unit)

            SocketSingleton.emit(action,socketAction)
        }

        fun joinGame(gameId: String, uid: String){
            val route = GameEvent.JOIN_GAME.event
            val addPlayerRequest = AddPlayerRequest(gameId,uid)
            val socketAction = SocketAction(route,addPlayerRequest)
            SocketSingleton.emit(action,socketAction)
        }
    }
}

data class Route(val route: String)