package com.example.polychat.models

import com.example.polychat.R

enum class Sounds(val id: Int) {
    WORD_GUESSED(R.raw.msn),
    UNCAPPED_PEN(R.raw.uncappedpen),
    SENT(R.raw.sent),
    NEW_MESSAGE(R.raw.sent),
    NOTIFICATION(R.raw.message_notification),
    DRAW(R.raw.draw),
    VICTORY(R.raw.victory)
}