package com.example.polychat.models.userModels

data class UserGameStats (
        val rounds_played: Int,
        val victories: Int,
        val failures: Int,
        val rounds_avg_time: Int,
        val total_time_played: Int,
        val total_games_played: Int
)