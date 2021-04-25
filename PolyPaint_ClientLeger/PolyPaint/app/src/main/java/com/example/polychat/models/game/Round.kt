package com.example.polychat.models.game

data class Round(
    val _id: String,
    val player_guessing_id: String,
    val player_drawing_id: String,
    val team_win_id: String,
    val points_won: Int,
    val canvas: Canvas,
    val game_img: GameImage,
    val reply_team_id: String?,
    val reply_player1_guessing_id: String?,
    val reply_player2_guessing_id: String?
)