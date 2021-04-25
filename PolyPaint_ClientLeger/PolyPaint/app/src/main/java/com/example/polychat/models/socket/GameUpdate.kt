package com.example.polychat.models.socket

enum class GameUpdate(val value: Int){
    DEFAULT(0),
    CREATED (1),
    UPDATED (2),
    USER_JOIN (3),
    USER_QUIT (4),
    VIRTUAL_JOIN (5),
    ACTION_FLOW (6)
}