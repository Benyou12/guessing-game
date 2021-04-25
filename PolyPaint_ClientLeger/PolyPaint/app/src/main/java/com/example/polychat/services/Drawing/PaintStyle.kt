package com.example.polychat.services.Drawing

import android.graphics.Paint

class PaintStyle {
    companion object {

        const val DEFAULT_WIDTH = 5f

        fun newInstance(color: Int, cap: Paint.Cap, width: Float): Paint {
            return Paint().apply {
                this.color = color
                style = Paint.Style.STROKE
                strokeJoin = Paint.Join.ROUND
                strokeCap = cap
                strokeWidth = width
                isAntiAlias = true
            }
        }

        fun clone(paint: Paint): Paint {
            return Paint().apply {
                color = paint.color
                style = paint.style
                strokeJoin = paint.strokeJoin
                strokeCap = paint.strokeCap
                strokeWidth = paint.strokeWidth
                isAntiAlias = true
            }
        }
    }
}