package com.example.polychat.framgmentsViews

import android.graphics.Color
import android.graphics.Paint
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.SeekBar
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment
import com.example.polychat.R
import com.example.polychat.models.game.Teams
import com.example.polychat.services.util.Localization
import com.example.polychat.views.CanvasView
import kotlinx.android.synthetic.main.layout_canvas.*
import kotlinx.android.synthetic.main.layout_canvas.view.*
import kotlinx.android.synthetic.main.layout_canvas_navbar.view.*

class CanvasFragment(private val canvasId: String, private val teams: Teams, private val wordToDraw: String): Fragment() {
    companion object{
        fun newInstance(canvasId: String, teams: Teams, wordToDraw: String): CanvasFragment {
            return CanvasFragment(canvasId, teams, wordToDraw)
        }
    }

    override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.layout_canvas, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        setColorButtonsListener(view)
        setActionButtonsListener(view)
        view.input_guess.alpha = 0f
//        setLanguage(view)
        listen(view.canvas_view)
    }

    private fun setLanguage(view: View) {
        Localization.setButton(view.button_clear,view.button_undo,button_eraser)
        Localization.setTextView(view.team_label,view.opponent_label,view.word_to_draw,view.player_role_action)
    }

    private fun setColorButtonsListener(view: View) {
        view.button_black_color.setOnClickListener {
            view.canvas_view.color = Color.BLACK
            view.canvas_view.drawMode()
        }
        view.button_red_color.setOnClickListener {
            view.canvas_view.color = ContextCompat.getColor(view.context,R.color.red)
            view.canvas_view.drawMode()
        }
        view.button_yellow_color.setOnClickListener {
            view.canvas_view.color = ContextCompat.getColor(view.context,R.color.yellow)
            view.canvas_view.drawMode()
        }
        view.button_green_color.setOnClickListener {
            view.canvas_view.color = ContextCompat.getColor(view.context, R.color.green)
            view.canvas_view.drawMode()
        }
        view.button_blue_color.setOnClickListener {
            view.canvas_view.color = ContextCompat.getColor(view.context,R.color.blue)
            view.canvas_view.drawMode()
        }
        view.button_pink_color.setOnClickListener {
            view.canvas_view.color = ContextCompat.getColor(view.context,R.color.pink)
            view.canvas_view.drawMode()
        }
        view.button_square_cap.setOnClickListener {
            view.canvas_view.strokeCap = Paint.Cap.SQUARE
            view.canvas_view.drawMode()
        }
        view.button_round_cap.setOnClickListener {
            view.canvas_view.strokeCap = Paint.Cap.ROUND
            view.canvas_view.drawMode()
        }
        view.seekBar.setOnSeekBarChangeListener(object: SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar?, progress: Int, fromUser: Boolean) {
                view.canvas_view.strokeWidth = progress.toFloat()
            }

            override fun onStartTrackingTouch(seekBar: SeekBar?) {
            }

            override fun onStopTrackingTouch(seekBar: SeekBar) {
            }
        })
        view.team_score.text = teams.userTeam.score.toString()
        view.oponnente_score.text = teams.opponent.score.toString()
        view.word_to_draw.text = wordToDraw
        view.button_eraser.setOnClickListener {
            canvas_view.eraseMode()
        }
        view.button_reply.visibility = View.GONE
    }

    private fun setActionButtonsListener(view: View) {
        view.button_undo.setOnClickListener { view.canvas_view.undo()}
        view.button_clear.setOnClickListener {
            view.canvas_view.clearCanvas()
            view.canvas_view.drawMode()
        }
    }

    private fun listen(canvas: CanvasView) {
        canvas.startDrawing(canvasId)
    }
}