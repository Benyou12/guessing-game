package com.example.polychat.services.game

import com.example.polychat.models.Drawing.Stroke
import com.example.polychat.models.game.GameActions
import com.example.polychat.models.game.Action
import com.example.polychat.models.socket.AddPlayerRequest
import com.example.polychat.models.socket.GameEvent
import com.example.polychat.models.socket.SocketAction
import com.example.polychat.models.socket.SocketEvents
import com.example.polychat.services.serverServices.SocketSingleton

data class Game(val gid: String)


data class DrawingMove(
    val strokes: ArrayList<Stroke>
)

class GameService {


    companion object {
        //todo move this to socket class
        private val action = "action"

        fun sendStrok(stroke: Stroke){
            val route = SocketEvents.SEND_STROKE.event
            val socketAction = SocketAction(route, stroke)
            SocketSingleton.emit(action, socketAction)
        }

        fun addVirtualPalyer(gameID: String){
            val route = GameEvent.ADD_VIRTUAL_PLAYER.event
            val addVirtualPlayerRequest = AddVirtualPlayerRequest(gameID)
            val socketAction = SocketAction(route,addVirtualPlayerRequest)
            SocketSingleton.emit(action,socketAction)
        }

        fun startGame(gameID: String, userID: String){
            val route = GameEvent.GAME_PATCH_ACTION.event
            val gameAction = Action(gameID,GameActions.START.value,null,null,userID,null)
            val socketAction = SocketAction(route,gameAction)
            SocketSingleton.emit(action,socketAction)
        }

        fun quitQame(gameID: String, userID: String){
            val route = GameEvent.GAME_QUIT.event
            val payload = AddPlayerRequest(gameID,userID)
            val socketAction = SocketAction(route,payload)
            SocketSingleton.emit(action,socketAction)
        }

        fun delteGame(gameID: String){
            val route = GameEvent.GAME_DELETE.event
            val socketAction = SocketAction(route,DeleteGame(gameID))
            SocketSingleton.emit(action,socketAction)
        }

        fun emitGuessedWord(userID: String, gameID: String, roundID: String, word: String){
            val route = GameEvent.GAME_PATCH_ACTION.event
            val payload = ActionWordGuess(word)
            val gameAction = Action(
                    gameID,
                    GameActions.GUESS_WORD.value,
                    roundID,
                    null,
                    userID,payload)
            val socketAction = SocketAction(route,gameAction)
            SocketSingleton.emit(action,socketAction)
        }
    }
data class AddVirtualPlayerRequest(val _id: String)

data class Uids(val uids: ArrayList<String>)

data class  ActionWordGuess(val word: String)

data class DeleteGame(val _id: String)
}