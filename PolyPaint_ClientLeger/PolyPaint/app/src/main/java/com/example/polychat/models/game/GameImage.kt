package com.example.polychat.models.game

data class GameImage(
    val _id: String,
    val hints: ArrayList<String>,
    val word: String,
    val difficulty: GAME_DIFFICULTY,
    val svg_link: String?,
    val drawing_mode: DRAWING_MODE,
    val lang: Int?
)

enum class GAME_DIFFICULTY(val value: Int){
    EASY (0),
    MEDIUM(1),
    HARD (2)
}

enum class DRAWING_MODE(val value: Int){
    RANDOM(0),
    CENTERED(1),
    PANORAMIC(2)
}