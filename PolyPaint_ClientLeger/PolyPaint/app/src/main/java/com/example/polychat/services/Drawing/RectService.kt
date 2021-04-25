package com.example.polychat.services.Drawing

import android.graphics.Path
import android.graphics.Rect
import com.example.polychat.models.Drawing.Position
import android.graphics.PathMeasure

class RectService {
    companion object {
        const val MAX_WIDTH: Float = 20f
        const val MIN_WIDTH: Float = 5f

        fun getPoints(path: Path, steps: Int): ArrayList<Position> {
            val points = arrayListOf<Position>()
            val pm = PathMeasure(path, false)
            val length = pm.length
            var portionCovered = 0f
            val speed = length / steps
            var counter = 0
            val aCoords = FloatArray(2)
            aCoords[0] = 0f
            aCoords[1] = 0f
            while (portionCovered < length && counter < steps) {
                pm.getPosTan(portionCovered, aCoords , null)
                points.add(Position(aCoords[0], aCoords[1]))
                counter++
                portionCovered += speed
            }

            return points
        }

        fun adjust(rect: Rect): Rect {
            if( rect.left > rect.right)
            {
                val temp = rect.right
                rect.right = rect.left
                rect.left = temp
            }
            if(rect.top > rect.bottom)
            {
                val temp = rect.bottom
                rect.bottom = rect.top
                rect.top = temp
            }
            return rect
        }
    }
}
