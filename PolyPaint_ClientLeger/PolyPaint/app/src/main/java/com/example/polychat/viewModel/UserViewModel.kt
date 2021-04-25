package com.example.polychat.viewModel

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.polychat.models.User
import com.example.polychat.models.socket.IOAuthResponse
import com.example.polychat.models.socket.SocketEvents
import com.example.polychat.models.userModels.UserGamification
import com.example.polychat.models.userModels.UserStats
import com.example.polychat.services.serverServices.SocketSingleton
import com.google.gson.GsonBuilder

class UserViewModel : ViewModel(){
    private val _usersInConversation = MutableLiveData<Array<User>>()
    private val _allUsers = MutableLiveData<Array<User>>()
    private val _onlineUsers = MutableLiveData<Array<User>>()
    private val _userOAuth = MutableLiveData<IOAuthResponse>()
    private val _publicUsers = MutableLiveData<Array<UserStats>>()
    private val _userStats = MutableLiveData<UserStats>()
    private val _logOut = MutableLiveData<User>()

    val allUsers: LiveData<Array<User>>
        get() = _allUsers
    val usersInConversation: LiveData<Array<User>>
        get() = _usersInConversation
    val onlineUsers: LiveData<Array<User>>
        get() = _onlineUsers
    val userOAuth: LiveData<IOAuthResponse>
        get() = _userOAuth
    val publicUsers: LiveData<Array<UserStats>>
        get() = _publicUsers
    val userStats: LiveData<UserStats>
        get() = _userStats
    val logOut: LiveData<User>
        get() = _logOut

    init {
        onUsersInConversations()
        onAllUsers()
        onOnlineUsers()
        onPublicUsers()
        onOAuthSignin()
        onUserStats()
        onLogOutReceived()
    }

    private fun onLogOutReceived() {
        SocketSingleton.socket.on(SocketEvents.USER_LOG_OUT.event){
            val userStats = GsonBuilder().create().fromJson(it[0].toString(),User::class.java)
            _logOut.postValue(userStats)
        }
    }

    private fun onUserStats() {
        SocketSingleton.socket.on(SocketEvents.USER_ONE.event){
            val userStats = GsonBuilder().create().fromJson(it[0].toString(),UserStats::class.java)
            _userStats.postValue(userStats)
        }
    }

    private fun onPublicUsers() {
        SocketSingleton.socket.on(SocketEvents.ALL_USERS.event){
            val response = GsonBuilder().create().fromJson(it[0].toString(),Array<UserStats>::class.java)
            _publicUsers.postValue(response)
        }
    }

    private fun onOAuthSignin() {
        SocketSingleton.socket.on(SocketEvents.USER_OAUTH.event){
            val oAuthResponse = GsonBuilder().create().fromJson(it[0].toString(),IOAuthResponse::class.java)
            _userOAuth.postValue(oAuthResponse)
        }
    }

    private fun onOnlineUsers() {
        SocketSingleton.socket.on(SocketEvents.USER_ONLINE.event){
            val response = GsonBuilder().create().fromJson(it[0].toString(),Array<User>::class.java)
            _onlineUsers.postValue(response)
        }
    }

    private fun onAllUsers() {
        SocketSingleton.socket.on(SocketEvents.ALL_USERS.event){
            val users: Array<User> = GsonBuilder().create().fromJson(it[0].toString(), Array<User>::class.java)
            _allUsers.postValue(users)
        }
    }

    private fun onUsersInConversations() {
        SocketSingleton.socket.on(SocketEvents.USER_CONVERSATION.event){
            val users: Array<User> = GsonBuilder().create().fromJson(it[0].toString(), Array<User>::class.java)
            _usersInConversation.postValue(users)
        }
    }
}