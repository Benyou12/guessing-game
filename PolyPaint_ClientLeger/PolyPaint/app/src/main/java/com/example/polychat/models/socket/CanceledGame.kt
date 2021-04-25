package com.example.polychat.models.socket

data class CanceledGame(
    val _id: String,
    val desc: CanceledGameDescription
)

data class CanceledGameDescription(
    val fr: String,
    val en: String
)
