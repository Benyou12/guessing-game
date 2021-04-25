package com.example.polychat.models.game

data class Team(
    var playerOne: Player?,
    var playerTwo: Player?,
    val score: Int
)

data class Teams(
        val userTeam: Team,
        val opponent:Team
)