package com.example.polychat.viewModel

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.polychat.models.Message
import com.example.polychat.models.socket.NewMessage
import com.example.polychat.models.socket.SocketEvents
import com.example.polychat.services.serverServices.SocketSingleton
import com.google.gson.GsonBuilder

class MessagesViewModel : ViewModel(){
    private val _newMessage = MutableLiveData<NewMessage>()
    private val _messages = MutableLiveData<Array<Message>>()

    val messages: LiveData<Array<Message>>
        get() = _messages
    val newMessage: LiveData<NewMessage>
        get() = _newMessage


    init {
        onNewMessage()
        onMessages()
    }

    private fun onMessages() {
        SocketSingleton.socket.on(SocketEvents.MESSAGE_ALL.event){
            val messages: Array<Message> = GsonBuilder().create().fromJson(it[0].toString(), Array<Message>::class.java)
            _messages.postValue(messages)
        }
    }

    private fun onNewMessage() {
        SocketSingleton.socket.on(SocketEvents.MESSAGE_NEW.event){
            val serverNewMessage: NewMessage = GsonBuilder().create().fromJson(it[0].toString(), NewMessage::class.java)
            _newMessage.postValue(serverNewMessage)
        }
    }
}