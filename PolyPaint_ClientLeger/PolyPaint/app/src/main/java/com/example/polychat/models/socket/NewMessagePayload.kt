package com.example.polychat.models.socket

data class SocketMessage(val text: String, val user: Uid)
data class NewMessagePayload(val cid: String, val message: SocketMessage)