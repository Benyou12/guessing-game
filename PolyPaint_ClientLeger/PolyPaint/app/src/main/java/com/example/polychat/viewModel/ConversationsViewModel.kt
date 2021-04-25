package com.example.polychat.viewModel

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.polychat.models.Conversation
import com.example.polychat.models.User
import com.example.polychat.models.socket.InviteUserResponse
import com.example.polychat.models.socket.SocketEvents
import com.example.polychat.services.serverServices.SocketSingleton
import com.google.gson.GsonBuilder

class ConversationsViewModel : ViewModel () {

    private val _conversations = MutableLiveData<Array<Conversation>>()
    private val _conversation = MutableLiveData<Conversation>()
    private val _newConversation = MutableLiveData<Conversation>()
    private val _searchConversationsResponse = MutableLiveData<Array<Conversation>>()
    private val _conversationUpdated = MutableLiveData<Conversation>()
    private val _conversationInvite = MutableLiveData<InviteUserResponse>()

    val newConversation: LiveData<Conversation>
        get() = _newConversation
    val conversation: LiveData<Conversation>
        get() = _conversation
    val conversations: LiveData<Array<Conversation>>
        get() = _conversations
    val searchConversationsResponse: LiveData<Array<Conversation>>
        get() = _searchConversationsResponse
    val conversationUpdated: LiveData<Conversation>
        get() = _conversationUpdated
    val inviteUserResponse: LiveData<InviteUserResponse>
        get() = _conversationInvite

    init {
        listenOnConversations()
        listenOnConversation()
        listenOnNewConversation()
        listenToSearchEvent()
        listenToConversationUpdated()
        listenOnInvitation()
    }


    private fun listenOnInvitation() {
        SocketSingleton.socket.on(SocketEvents.CONVERSATION_INVITE.event){
            val response = GsonBuilder().create().fromJson(it[0].toString(),InviteUserResponse::class.java)
            _conversationInvite.postValue(response)
        }
    }

    private fun listenToConversationUpdated() {
            SocketSingleton.socket.on(SocketEvents.CONVERSATION_UPDATE.event){
                val conversation: Conversation = GsonBuilder().create().fromJson(it[0].toString(), Conversation::class.java)
                _conversationUpdated.postValue(conversation)
            }
    }

    private fun listenToSearchEvent() {
        SocketSingleton.socket.on(SocketEvents.SEARCH_RESPONSE.event){
            val response: Array<Conversation> = GsonBuilder().create().fromJson(it[0].toString(), Array<Conversation>::class.java)
            _searchConversationsResponse.postValue(response)
        }
    }

    private fun listenOnNewConversation() {
        SocketSingleton.socket.on(SocketEvents.CONVERSATION_NEW.event){
            val conversation: Conversation = GsonBuilder().create().fromJson(it[0].toString(), Conversation::class.java)
            _newConversation.postValue(conversation)
        }
    }

    private fun listenOnConversation() {
        SocketSingleton.socket.on(SocketEvents.CONVERSATION_ONE.event){
            val conversation: Conversation = GsonBuilder().create().fromJson(it[0].toString(), Conversation::class.java)
            _conversation.postValue(conversation)
        }
    }

    private fun listenOnConversations(){
        SocketSingleton.socket.on (SocketEvents.CONERSATION_ALL.event){
            val serverConversations: Array<Conversation> = GsonBuilder().create().fromJson(it[0].toString(), Array<Conversation>::class.java)
            _conversations.postValue(serverConversations)
        }
    }

}