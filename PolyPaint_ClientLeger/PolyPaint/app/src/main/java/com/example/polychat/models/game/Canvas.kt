package com.example.polychat.models.game

import com.example.polychat.models.Drawing.Stroke

data class Canvas(
    val _id: String,
    val strokes: ArrayList<Stroke>,
    val uids: ArrayList<String> /// just for test
)