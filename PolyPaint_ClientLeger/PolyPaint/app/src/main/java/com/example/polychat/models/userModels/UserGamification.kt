package com.example.polychat.models.userModels

data class UserGamification(
        val badges: ArrayList<UserBadge>,
        val level: Level,
        val points: Int
)