package com.example.polychat.views

import android.content.Context
import android.graphics.*
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.appcompat.app.AppCompatDelegate
import com.example.polychat.models.Drawing.Cap
import com.example.polychat.models.Drawing.Position
import com.example.polychat.models.Drawing.Shape
import com.example.polychat.models.Drawing.Stroke
import com.example.polychat.models.Sounds
import com.example.polychat.services.Drawing.PaintStyle
import com.example.polychat.services.Drawing.RectService
import com.example.polychat.services.game.GameService
import com.example.polychat.services.util.Sound
import okhttp3.internal.toHexString
import kotlin.collections.ArrayList

class CanvasView(context: Context, attrs: AttributeSet) : View(context, attrs) {

    var id: String = ""
    private var current = Position(0f, 0f)
    private var start = Position(0f, 0f)

    var color = Color.BLACK
    var strokeCap = Paint.Cap.ROUND
    var strokeWidth = PaintStyle.DEFAULT_WIDTH
    private var path = Path()
    private var paint = PaintStyle.newInstance(color, strokeCap, strokeWidth)
    private var shapes: ArrayList<Shape> = ArrayList()
    private var rects: ArrayList<Rect> = arrayListOf()
    private var stroke: Stroke
    private var isEraseMode: Boolean = false

    init {
        val hexColor = "#" + color.toHexString()
        val cap = Stroke.toServerCapType(strokeCap)
        stroke = Stroke.newInstance(null, id, hexColor, cap, strokeWidth.toInt(), false)
        val elem = Shape(stroke._id, path, paint, rects)
        shapes.add(elem)
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        val isDarkMode = AppCompatDelegate.getDefaultNightMode() == AppCompatDelegate.MODE_NIGHT_YES
        shapes.forEach {
            canvas.drawPath(it.path, defineStyle(it.paint, isDarkMode))
        }
        GameService.sendStrok(stroke)
        stroke.coordinates = arrayListOf()
    }

    fun eraseMode() {
        isEraseMode = true
    }

    fun drawMode() {
        isEraseMode = false
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

    override fun onTouchEvent(event: MotionEvent?): Boolean {
        event ?: return false
        val touchX = event.x
        val touchY = event.y
        if(isEraseMode)
        {
            eraseModeActions(event.action, touchX, touchY)
        }
        else
        {
            drawModeActions(event.action, touchX, touchY)
        }
        invalidate()
        return true
    }

    private fun eraseModeActions(action: Int, x: Float, y: Float) {
        when (action)
        {
            MotionEvent.ACTION_DOWN -> eraseSegmentAction(x, y)
            MotionEvent.ACTION_MOVE -> eraseSegmentAction(x, y)
        }
    }

    private fun drawModeActions(action: Int, x: Float, y: Float) {
        when (action)
        {
            MotionEvent.ACTION_DOWN -> actionDown(x, y)
            MotionEvent.ACTION_MOVE -> actionMove(x, y)
            MotionEvent.ACTION_UP -> actionUp()
            MotionEvent.ACTION_CANCEL -> actionCancel(x, y)
        }
    }

    private fun eraseSegmentAction(x: Float, y: Float) {
        for (shape in shapes)
        {
            for(rect in shape.rects)
            {
                if (rect.contains(x.toInt(), y.toInt())) {
                    shapes.remove(shape)
                    val strokeToDelete = Stroke
                            .newInstance(
                                _id = shape.id,
                                canvas_id = id,
                                color = "#" + color.toHexString(),
                                cap = Cap.ROUND,
                                size = strokeWidth.toInt(),
                                toDelete = true)
                    GameService.sendStrok(strokeToDelete)
                    return
                }
            }
        }
    }
    
    private fun actionDown(x: Float, y: Float) {
        path.moveTo(x, y)
        setAllPositionsTo(x, y)
        addShape(color)
    }

    private fun actionMove(x: Float, y: Float) {
        path.lineTo(x , y)
        stroke.addPixel(x,y)
        setAllPositionsTo(x,y)
    }

    private fun actionUp() {
        path.lineTo(current.x, current.y)
        addRectsFromPoints()
    }

    private fun addRectsFromPoints() {
        val points = RectService.getPoints(path, steps = 1000)
        val offset = getRectOffset(strokeWidth)
        for(i in 0 until points.size)
        {
            val j = if(i-1 >= 0) i-1 else i
            val l = (points[j].x - offset).toInt()
            val t = (points[j].y - offset).toInt()
            val r = (points[i].x + offset).toInt()
            val b = (points[i].y + offset).toInt()
            rects.add(RectService.adjust(Rect(l, t, r, b)))
        }
    }

    private fun getRectOffset(width: Float): Float {
        return when {
            width < RectService.MIN_WIDTH -> RectService.MIN_WIDTH
            width < RectService.MAX_WIDTH -> width * 3 / 4
            width > RectService.MAX_WIDTH -> width * 1 / 2
            else -> width
        }
    }

    private fun actionCancel(x: Float, y: Float) {
        path.lineTo(x, y)
        stroke.addPixel(x,y)
    }

    /* reset positions to current position*/
    private fun setAllPositionsTo(x: Float, y: Float) {
        current.setPosition(x,y)
        start = current
    }

    private fun addShape(color: Int) {
        Sound.play(this.context, Sounds.DRAW, 0)
        val hexColor = "#" + color.toHexString()
        val cap = Stroke.toServerCapType(strokeCap)
        stroke = Stroke.newInstance(null, id, hexColor, cap, strokeWidth.toInt(), toDelete = false)
        paint = PaintStyle.newInstance(color, strokeCap, strokeWidth)
        path = Path()
        path.moveTo(current.x,current.y)
        rects = arrayListOf()
        val elem = Shape(stroke._id , path, paint, rects)
        shapes.add(elem)
    }

    fun undo() {
        shapes.removeAt(shapes.size - 1)
        if (shapes.size == 0)
        {
            path.reset()
            val shape = Shape(stroke._id, path, paint, rects)
            shapes.add(shape)
        }
        invalidate()
    }

    fun clearCanvas() {
        stroke.coordinates.clear()
        shapes = ArrayList()
        path.reset()
        rects = arrayListOf()
        val shape = Shape(stroke._id, path, paint, rects)
        shapes.add(shape)
        invalidate()
    }


    fun startDrawing(canvas_id: String) {
        id = canvas_id
        val cap = Stroke.toServerCapType(strokeCap)
        stroke = Stroke
                .newInstance(
                        null,
                        canvas_id,
                        "#" + color.toHexString(),
                        cap,
                        strokeWidth.toInt(),
                        toDelete = false)
    }
}
