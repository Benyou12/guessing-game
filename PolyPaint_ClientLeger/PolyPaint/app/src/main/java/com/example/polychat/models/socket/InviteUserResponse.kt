package com.example.polychat.models.socket

import com.example.polychat.models.Conversation
import com.example.polychat.models.Message
import com.example.polychat.models.User

data class InviteUserResponse(
    val conversation: Conversation,
    val messages: ArrayList<Message>,
    val uid: String,
    val user: User
)
