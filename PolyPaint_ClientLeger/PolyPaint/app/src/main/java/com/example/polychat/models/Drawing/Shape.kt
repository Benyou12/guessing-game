package com.example.polychat.models.Drawing

import android.graphics.Paint
import android.graphics.Path
import android.graphics.Rect

class SimpleShape(
        val id: String,
        val path: Path,
        val paint: Paint
)

class Shape(
        val id: String,
        val path: Path,
        val paint: Paint,
        var rects: ArrayList<Rect>
)