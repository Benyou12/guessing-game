package com.example.polychat.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Path
import android.util.AttributeSet
import android.view.View
import androidx.appcompat.app.AppCompatDelegate
import com.example.polychat.models.Drawing.Cap
import com.example.polychat.models.Drawing.SimpleShape
import com.example.polychat.models.Drawing.Stroke
import com.example.polychat.services.Drawing.PaintStyle
import kotlin.collections.ArrayList

class GuesserCanvasView(context: Context, attrs: AttributeSet) : View(context, attrs) {

    var id: String = ""
    private var isAuthorizeToGuess = false
    var shapes: ArrayList<SimpleShape> = ArrayList()
    private var lastStrokeReceived: Stroke? = null
    private lateinit var lastReceivedPath: Path
    private var stroke: Stroke? = null

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        val isDarkMode = AppCompatDelegate.getDefaultNightMode() == AppCompatDelegate.MODE_NIGHT_YES
        shapes.forEach {
            canvas.drawPath(it.path, defineStyle(it.paint, isDarkMode))
        }
        if(stroke == null) return
        stroke!!.coordinates = arrayListOf()
    }

    private fun defineStyle(p: Paint,isDarkMode: Boolean): Paint {
        return if (isDarkMode&& p.color == Color.BLACK) {
            val pt = PaintStyle.clone(p)
            pt.color = Color.WHITE
            pt
        } else {
            p
        }
    }

    fun strokesToPath(stroke: Stroke) {
        if(stroke.coordinates.size == 0) return
        val pixels = stroke.coordinates
        if(lastStrokeReceived == null || lastStrokeReceived!!._id != stroke._id)
        {
            lastReceivedPath = Path()
                    .apply {
                        moveTo(pixels.first().x,
                               pixels.first().y)
                    }
            lastStrokeReceived = stroke
            val strokeColorInt = if(stroke.color.length >= 7) Color.parseColor(stroke.color) else Color.BLACK
            val paint = PaintStyle
                    .newInstance(
                            strokeColorInt,
                            stroke.toAndroidCapType(),
                            stroke.size.toFloat())
            shapes.add(SimpleShape(stroke._id, lastReceivedPath,paint))
        }
        val lastIndex = stroke.coordinates.size - 1
        for (i in 0..lastIndex)
        {
            lastReceivedPath.lineTo(pixels[i].x, pixels[i].y)
        }
        invalidate()
    }

    fun authorizeToGuess(canvas_id: String) {
        id = canvas_id
        stroke = Stroke.newInstance(null, id, "#000", Cap.SQUARE, 5, false)
        isAuthorizeToGuess = true
    }

    fun deleteShape(shapeIdToDelete: String) {
        for(simpleShape in shapes)
        {
            if (simpleShape.id == shapeIdToDelete)
            {
                shapes.remove(simpleShape)
                invalidate()
                return
            }
        }
    }

}
