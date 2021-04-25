package com.example.polychat.models.game

data class Action(
    val game_id: String,
    val type: Int,
    val round_id: String?,
    val team_id: String?,
    val user_id: String,
    val payload: Any?  // todo or ActionWordGuess or IActionAddPlayer???
)

 interface ActionWordGuess {
    val word: String
}

interface ActionAddPlayer {
    val uid: String
    val isBot: Boolean
}