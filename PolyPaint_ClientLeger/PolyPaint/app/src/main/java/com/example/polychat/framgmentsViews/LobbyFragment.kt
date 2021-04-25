package com.example.polychat.framgmentsViews

import android.graphics.Color
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.polychat.R
import com.example.polychat.models.Label
import com.example.polychat.models.User
import com.example.polychat.models.game.Player
import com.example.polychat.models.game.Team
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.Labels
import com.example.polychat.services.game.GameService
import com.example.polychat.services.util.Localization
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.layout_game_lobby.view.*
import kotlinx.android.synthetic.main.layout_player_row_lobby.view.*
import okhttp3.internal.wait

const val MINIMUM_TO_REQUEST_VP = 2
const val MAX_PLAYERS = 4

val startButton = Label(en = "start the game", fr = "Commencer le jeu")
val virtButton = Label(en = "add virtual Player", fr = "Ajouter un joeur virtuel")
val waitButton = Label(en = "waiting ...", fr = "en attent ...")
fun setImageUrl(url: String, imageView: ImageView) {
    Picasso.get().load(url).into(imageView)
}

class PlayerRow(
        private val image: ImageView,
        private val name: TextView
) {
    fun setView(data: Player?) {
        data ?: return
            setImageUrl(data.user.profileImgUrl, image)
            name.text = data.user.username
    }
}

class LobbyViews (
        private val title: TextView,
        private val playerOne: PlayerRow,
        private val playerTwo: PlayerRow,
        private val playerThree: PlayerRow,
        private val playerFour: PlayerRow,
        private val startGameButton: Button
) {
    fun setView(data: LobbyData) {
        title.text = data.title
        playerOne.setView(data.teamOne.playerOne)
        playerTwo.setView(data.teamOne.playerTwo)
        playerThree.setView(data.teamTwo.playerOne)
        playerFour.setView(data.teamTwo.playerTwo)
        startGameButton.text = data.startButtonName
    }
}

data class LobbyData(
        val game_id: String,
        val game_name: String,
        val currentUser: User,
        val title: String,
        val teamOne: Team,
        val teamTwo: Team,
        var startButtonName: String
){
    fun numberOfPlayers(): Int {
        var count = 0
        if(teamOne.playerOne != null) count++
        if(teamOne.playerTwo != null) count++
        if(teamTwo.playerOne != null) count++
        if(teamTwo.playerTwo != null) count++
        startButtonName = when
        {
            count == 4 -> startButton.getValue()
            count < 2 -> waitButton.getValue()
            count >= 2 -> virtButton.getValue()
            else -> waitButton.getValue()
        }

        return count
    }
}

class LobbyFragment(private val data: LobbyData) : Fragment() {

    companion object{
        fun newInstance(data: LobbyData): LobbyFragment {
            return LobbyFragment(data)
        }
    }
    override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.layout_game_lobby, container, false)
    }

    override fun onViewCreated(v: View, savedInstanceState: Bundle?) {
        super.onViewCreated(v, savedInstanceState)
        v.textView_lobby_title.text = data.game_name
        v.lobby_message.text = Labels.LOBBY_MESSAGE.getValue()
        val playerOneRow = PlayerRow(
            v.player_row1.imageView_player,
            v.player_row1.textView_player_name
        )
        val playerTwoRow = PlayerRow(
            v.player_row2.imageView_player,
            v.player_row2.textView_player_name
        )
        val playerThreeRow = PlayerRow(
            v.player_row3.imageView_player,
            v.player_row3.textView_player_name
        )
        val playerFourRow = PlayerRow(
            v.player_row4.imageView_player,
            v.player_row4.textView_player_name
        )
        val title = v.textView_lobby_title
        val start = v.button_start_game
        val numberOfPlayers = data.numberOfPlayers()
        start.text = getButtonName(numberOfPlayers)
        if (data.teamOne.playerOne!!.user_id == CurrentUser.user.uid)
        {
            start.setOnClickListener {
                if(data.numberOfPlayers() >= MINIMUM_TO_REQUEST_VP) {
                    if (data.numberOfPlayers() < MAX_PLAYERS) {
                        GameService.addVirtualPalyer(data.game_id)
                    }
                    if (data.numberOfPlayers() == MAX_PLAYERS) {
                        GameService.startGame(data.game_id, data.currentUser.uid)
                    }
                }
            }
        }
        else
        {
            start.setBackgroundColor(Color.parseColor("#aaaaaa"))
        }
        val lobbyViews = LobbyViews(
            title,
            playerOneRow,
            playerTwoRow,
            playerThreeRow,
            playerFourRow,
            start
        )
        lobbyViews.setView(data)
    }

    private fun getButtonName(numberOfPlayers: Int): String {
        return if (numberOfPlayers == 4)
        {
            startButton.getValue()
        }
        else
        {
            virtButton.getValue()
        }
    }
}
