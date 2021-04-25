package com.example.polychat.models.socket

data class SocketAction<T>(val route: String, val payload: T)
