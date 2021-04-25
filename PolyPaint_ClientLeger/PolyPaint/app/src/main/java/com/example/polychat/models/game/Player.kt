package com.example.polychat.models.game

import com.example.polychat.models.User

data class Player(
    val _id: String,
    val user_id: String,
    var user: User,
    val role: String,
    val isVirtual: Boolean
)

enum class PLAYER_ROLE(val value: String) {
    DRAW("DRAWER"),
    GUESS("GUESSER"),
    NONE("NONE"),
    NEUTRAL("neutral")
}
