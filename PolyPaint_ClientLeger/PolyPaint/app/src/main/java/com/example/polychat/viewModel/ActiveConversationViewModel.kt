package com.example.polychat.viewModel

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class ActiveConversationViewModel : ViewModel(){
    private var _activeConversationId =  MutableLiveData<String>()
    val activeConversationId: LiveData<String>
        get() =  _activeConversationId

    fun setConversation(conversationID: String){
        _activeConversationId.postValue(conversationID)
    }
}