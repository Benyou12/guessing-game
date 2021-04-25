package com.example.polychat.framgmentsViews

import android.annotation.SuppressLint
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.models.Conversation
import com.example.polychat.models.Label
import com.example.polychat.models.game.*
import com.example.polychat.services.CanvasHolder
import com.example.polychat.services.Labels
import com.example.polychat.services.game.GameService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.viewModel.CanvasViewModel
import com.example.polychat.views.GuesserCanvasView
import kotlinx.android.synthetic.main.layout_canvas_navbar.view.*
import kotlinx.android.synthetic.main.layout_guesser_canvas.view.*

data class PlayerActionMessage(val guesser: Label, val none: Label, val reply: Label)

class GuesserCanvasFragment(val data: GameData, private val currentPlayer: Player, private val teams: Teams) : Fragment(){

    private val playerActionMessage : PlayerActionMessage = PlayerActionMessage(
            guesser = Label(
                    en = "It's your turn to guess",
                    fr = "C'est ton tour"
            ),
            none = Label(
                    en = "It's not your turn. Please Wait",
                    fr = "Ce n'est pas ton tour"
            ),
            reply = Label(
                    en = "Submit",
                    fr = "Soumettre"
            )
    )
    private lateinit var thisView: View
    companion object{
        fun newInstance(data: GameData, currentPlayer: Player, teams: Teams): GuesserCanvasFragment {
            return GuesserCanvasFragment(data, currentPlayer, teams)
        }
        var role = PLAYER_ROLE.NEUTRAL
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.layout_guesser_canvas, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        setLanguage(view)
        listen(view.canvas_view)
        view.word_to_draw.alpha = 0f
        view.team_score.text = teams.userTeam.score.toString()
        view.oponnente_score.text = teams.opponent.score.toString()
        thisView = view
        updateView()
        setInputGuessHint(view)
    }

    private fun setInputGuessHint(view: View) {
        val hint = Labels.WORD_TO_GUESS_VIEW_HINT.getValue()
        val wordLang = if(data.rounds.last().game_img.lang == 0 ) Labels.WORD_TO_GUESS_FRANCAIS.getValue()
                            else Labels.WORD_TO_GUESS_ANGLAIS.fr
        view.input_guess.hint = hint + wordLang

    }

    private fun updateView() {
        if(role != PLAYER_ROLE.NEUTRAL)
        {
            updateViewAccordingToRole(thisView, role.value)
        }
        else
        {
            updateViewAccordingToRole(thisView, currentPlayer.role)
        }
    }

    private fun updateViewAccordingToRole(view: View, playerRole: String) {
        if(data.rounds.last().reply_team_id != null) view.canvas_view.shapes = CanvasHolder.shapes
        view.button_reply.visibility = View.VISIBLE
        if(playerRole == PLAYER_ROLE.GUESS.value)
        {
            view.player_role_action.text = getLabelForGuesserPlayer()
            view.button_reply.text = playerActionMessage.reply.getValue()
            view.button_reply.setOnClickListener {
                val word = view.input_guess.text.toString()
                GameService.emitGuessedWord(
                        currentPlayer.user_id,
                        data._id,
                        data.rounds.last()._id,
                        word)
            }
            CanvasHolder.shapes = view.canvas_view.shapes
        }
        else
        {
            view.player_role_action.text = getLabelForNoRolePlayer()
            view.input_guess.visibility = View.GONE
            view.button_reply.visibility = View.GONE
            CanvasHolder.shapes = view.canvas_view.shapes
        }
    }

    private fun getLabelForNoRolePlayer(): String {
        return if(Localization.langCode.code == LangCode.FR.code) {
            playerActionMessage.none.fr
        } else {
            playerActionMessage.none.en
        }
    }

    private fun getLabelForGuesserPlayer(): String {
        return if(Localization.langCode.code == LangCode.FR.code) {
            playerActionMessage.guesser.fr
        } else {
            playerActionMessage.guesser.en
        }
    }

    private fun setLanguage(view: View) {
        Localization.setTextView(view.player_role_action,view.word_to_draw,view.team_label,view.opponent_label)
        //Localization.setInput(view.input_guess)
    }

    private fun listen(canvas: GuesserCanvasView) {
        canvas.authorizeToGuess(data.rounds.last().canvas._id)
        CanvasViewModel().strokeUpdated.observe(this, Observer {stroke ->
            if(stroke.toDelete)
            {
                canvas.deleteShape(stroke._id)
            }
            else
            {
                canvas.strokesToPath(stroke)
            }
        })
    }
}