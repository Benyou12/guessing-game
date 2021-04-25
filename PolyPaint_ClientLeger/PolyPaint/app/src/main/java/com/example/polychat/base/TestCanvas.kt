package com.example.polychat.base

import android.graphics.Color
import android.graphics.Paint
import android.os.Bundle
import android.widget.SeekBar
import androidx.core.content.ContextCompat
import com.example.polychat.R
import kotlinx.android.synthetic.main.layout_canvas.*

class TestCanvas : BaseActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.layout_canvas)
        setBtnEvents()
        setEraser()
    }

    private fun setEraser() {
        button_eraser.setOnClickListener {
           canvas_view.eraseMode()
        }
    }

    private fun setBtnEvents() {
        val context = this
        button_black_color.setOnClickListener { canvas_view.color = Color.BLACK; canvas_view.drawMode() }
        button_red_color.setOnClickListener { canvas_view.color = ContextCompat.getColor(context,R.color.red); canvas_view.drawMode() }
        button_yellow_color.setOnClickListener {  canvas_view.color = ContextCompat.getColor(context,R.color.yellow); canvas_view.drawMode() }
        button_green_color.setOnClickListener { canvas_view.color = ContextCompat.getColor(context, R.color.green); canvas_view.drawMode() }
        button_blue_color.setOnClickListener { canvas_view.color = ContextCompat.getColor(context,R.color.blue); canvas_view.drawMode() }
        button_pink_color.setOnClickListener { canvas_view.color = ContextCompat.getColor(context,R.color.pink); canvas_view.drawMode() }
        button_square_cap.setOnClickListener { canvas_view.strokeCap = Paint.Cap.SQUARE; canvas_view.drawMode() }
        button_round_cap.setOnClickListener { canvas_view.strokeCap = Paint.Cap.ROUND; canvas_view.drawMode() }
        seekBar.setOnSeekBarChangeListener(object: SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar?, progress: Int, fromUser: Boolean) {
                canvas_view.strokeWidth = progress.toFloat()
            }

            override fun onStartTrackingTouch(seekBar: SeekBar?) {
            }

            override fun onStopTrackingTouch(seekBar: SeekBar) {
            }
        })

        button_undo.setOnClickListener { canvas_view.undo()}
        button_clear.setOnClickListener { canvas_view.clearCanvas(); canvas_view.drawMode()}
    }

}
