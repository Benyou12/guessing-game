package com.example.polychat.models.userModels

data class UserStats(
        val uid: String,
        val firstName: String,
        val lastName: String,
        var username: String,
        val email: String,
        val profileImgUrl: String,
        val game_stats: UserGameStats?,
        val game_history: ArrayList<UserGameStatsHistory>?,
        val auth_history: ArrayList<UserAuthStats>?,
        val gamification: UserGamification?,
        val isConnected: Boolean?
)

