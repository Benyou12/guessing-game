package com.example.polychat.models.game

enum class GameActions(val value: Int){
    REQUEST_HINT(0),
    GUESS_WORD(1),
    ROUND_WON(2),
    ADD_PLAYER(3),
    START(4),
    END(5),
    START_ROUND(6),
    END_ROUNT(7)
}