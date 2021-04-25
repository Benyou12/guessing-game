package com.example.polychat.services.serverServices

import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.polychat.models.User
import com.example.polychat.models.socket.*
import com.example.polychat.services.BASE_URL
import com.google.gson.GsonBuilder
import io.socket.client.IO
import io.socket.client.Socket
import android.icu.lang.UCharacter.GraphemeClusterBreak.T



object SocketSingleton {

    private const val action = "action"
    private const val socketURL = BASE_URL
    var socket: Socket
    var user: User? = null

    init {
        val opts = IO.Options()
        opts.reconnection = true
        socket = IO.socket(socketURL,opts)
        connect()
        onSocketDisconnected()
        onReconnect()
    }

    fun connect() {
        if (!socket.connected())
        {
            socket.connect()
            pingpong()
        }
    }

    fun isConnected(): Boolean = socket.connected()

    fun disconnect(){
        if(socket.connected())
        {
            socket.disconnect()
            Log.d("Disconnect", "Socket disconnected by user")
        }
    }

    fun <T>emit(event: String, content: T) {
        val jsonContent = GsonBuilder().create().toJson(content)
        socket.emit(event,jsonContent)
    }

    fun joinRoom(user: User, activity: AppCompatActivity)
    {
        emit(SocketEvents.JOIN_ROOM.event, Uid(user.uid))
        socket.on(SocketEvents.ROOM_JOINED.event){
            activity.runOnUiThread {
                val uid = GsonBuilder().create().fromJson(it[0].toString(), Uid::class.java)
                Toast.makeText(activity.baseContext, "Welcome : ${user.username}", Toast.LENGTH_SHORT).show()
            }
        }
    }

    fun pingpong()
    {
        socket.on(SocketEvents.PING.event){
            emit(SocketEvents.PONG.event, it[0].toString())
        }
    }

    private fun onSocketDisconnected() {
        socket.on("disconnect"){
            Log.d("Disconnect", "socket disconnected for weird reason")
            Log.d("Disconnect", "${it[0]}")
        }
    }

    private fun onReconnect() {
        socket.on("reconnect"){
            Log.d("reconnect", "${it[0]}")
            if(user == null) return@on
            emit(SocketEvents.JOIN_ROOM.event, Uid(user!!.uid))
        }
    }


}