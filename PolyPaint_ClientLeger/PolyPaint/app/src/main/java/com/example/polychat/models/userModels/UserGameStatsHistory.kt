package com.example.polychat.models.userModels

data class UserGameStatsHistory(
        val game_id: String,
        val timestamp: Long,
        val names: ArrayList<String>,
        val myTeamResult: Int,
        val otherTeamResult: Int
)