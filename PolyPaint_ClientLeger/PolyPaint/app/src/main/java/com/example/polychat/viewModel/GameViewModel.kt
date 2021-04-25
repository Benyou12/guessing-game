package com.example.polychat.viewModel

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.polychat.models.game.GameData
import com.example.polychat.models.socket.CanceledGame
import com.example.polychat.models.socket.GameEvent
import com.example.polychat.services.game.GameService
import com.example.polychat.services.serverServices.SocketSingleton
import com.google.gson.GsonBuilder

class GameViewModel : ViewModel(){
    private val _updatedGameData = MutableLiveData<GameData>()
    private val _createdGame = MutableLiveData<GameData>()
    private val _activeGames = MutableLiveData<Array<GameData>>()
    private val _allGames = MutableLiveData<Array<GameData>>()
    private val _deletedGame = MutableLiveData<GameService.DeleteGame>()
    private val _canceledGame = MutableLiveData<CanceledGame>()

    val updatedGameData: LiveData<GameData>
        get() = _updatedGameData
    val createdGameData: LiveData<GameData>
        get() = _createdGame
    val activesGameData: LiveData<Array<GameData>>
        get() = _activeGames
    val allGames: LiveData<Array<GameData>>
        get() = _allGames
    val deletedGame: LiveData<GameService.DeleteGame>
        get() = _deletedGame
    val canceledGame: LiveData<CanceledGame>
        get() = _canceledGame

    init {
        onGameJoinedReceived()
        onGameCreatedReceived()
        onActiveGameReceived()
        onAllGamesReceived()
        onGameDeleted()
        onCanceledGame()
    }

    private fun onCanceledGame() {
        SocketSingleton.socket.on(GameEvent.GAME_CANCELED.event){
            val response = GsonBuilder().create().fromJson(it[0].toString(), CanceledGame::class.java)
            _canceledGame.postValue(response)
        }
    }

    private fun onGameDeleted() {
        SocketSingleton.socket.on(GameEvent.GAME_DELETED.event){
            val response = GsonBuilder().create().fromJson(it[0].toString(), GameService.DeleteGame::class.java)
            _deletedGame.postValue(response)
        }
    }

    private fun onAllGamesReceived() {
        SocketSingleton.socket.on(GameEvent.ALL_GAME.event){
            val allGameData: Array<GameData> = GsonBuilder().create().fromJson(it[0].toString(), Array<GameData>::class.java)
            _allGames.postValue(allGameData)
        }
    }

    private fun onActiveGameReceived() {
        SocketSingleton.socket.on(GameEvent.ACTIVE_GAME.event){
            val activeGameData: Array<GameData> = GsonBuilder().create().fromJson(it[0].toString(), Array<GameData>::class.java)
            _activeGames.postValue(activeGameData)
        }
    }

    private fun onGameCreatedReceived() {
        SocketSingleton.socket.on(GameEvent.CREATED_GAME.event){
            val createdGameData: GameData = GsonBuilder().create().fromJson(it[0].toString(), GameData::class.java)
            _createdGame.postValue(createdGameData)
        }
    }

    private fun onGameJoinedReceived() {
        SocketSingleton.socket.on(GameEvent.GAME_UPDATED.event){
            val joinedGameData: GameData = GsonBuilder().create().fromJson(it[0].toString(), GameData::class.java)
            _updatedGameData.postValue(joinedGameData)
        }
    }
}