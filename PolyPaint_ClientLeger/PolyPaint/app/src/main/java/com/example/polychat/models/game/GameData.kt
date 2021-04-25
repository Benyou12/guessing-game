package com.example.polychat.models.game

data class GameData(
        val _id: String,
        val conversation_id: String,
        val state: Int,
        val isReady: Boolean,
        var teamOne: Team,
        var teamTwo: Team,
        val actions: ArrayList<Action>,
        val rounds: ArrayList<Round>,
        val max_rounds: Int,
        val timestamp: Long,
        val name: String,
        val updateAction: Int
)
