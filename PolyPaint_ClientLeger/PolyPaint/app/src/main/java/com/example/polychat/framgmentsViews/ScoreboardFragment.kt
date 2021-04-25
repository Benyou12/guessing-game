package com.example.polychat.framgmentsViews

import android.graphics.Color
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.polychat.R
import com.example.polychat.game.GameState
import com.example.polychat.models.Label
import com.example.polychat.models.Sounds
import com.example.polychat.models.game.Player
import com.example.polychat.models.game.Round
import com.example.polychat.models.game.Team
import com.example.polychat.services.Labels
import com.example.polychat.services.game.GameService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.Sound
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.layout_scoreboard.*
import kotlinx.android.synthetic.main.layout_scoreboard.view.*
import kotlinx.android.synthetic.main.layout_team_card.view.*
import nl.dionsegijn.konfetti.models.Shape
import nl.dionsegijn.konfetti.models.Size

class PlayerInfo(
    private val username: TextView,
    private val image: ImageView,
    private val role: TextView
) {
    fun setUsername(username: String) {
        this.username.text = username
    }

    fun setRole(role: String) {
        this.role.text = role
    }

    fun setPlayerImage(url: String) {
        Picasso.get().load(url).into(image)
    }
}

class TeamCard(
    val title: TextView,
    val playerOne: PlayerInfo,
    val playerTwo: PlayerInfo,
    val label: TextView,
    val score: TextView
) {

    fun setTeamData(teamData: TeamData) {
        setTitle(teamData.title)
        setLabel(teamData.label)
        setScore(teamData.score)
        setPlayerOne(teamData.playerOne)
        setPlayerTwo(teamData.playerTwo)
    }

    private fun setTitle(title: String) {
        this.title.text = title
    }

    private fun setPlayerOne(playerData: PlayerData) {
        playerOne.setUsername(playerData.username)
        playerOne.setRole(playerData.role)
        playerOne.setPlayerImage(playerData.imageUrl)
    }

    private fun setPlayerTwo(playerData: PlayerData) {
        playerTwo.setUsername(playerData.username)
        playerTwo.setRole(playerData.role)
        playerTwo.setPlayerImage(playerData.imageUrl)
    }

    private fun setLabel(scoreLabel: String){
        this.label.text = scoreLabel
    }

    private fun setScore(score: String){
        this.score.text = score
    }
}

data class PlayerData(
    val username: String,
    val role: String,
    val imageUrl: String
) {
    companion object{
        fun newInstanceFromPlayer(player: Player?): PlayerData {
            return PlayerData(player!!.user.username, player.role, player.user.profileImgUrl)
        }
    }
}
data class TeamData(
    val title: String,
    val playerOne: PlayerData,
    val playerTwo: PlayerData,
    val label: String,
    val score: String
)

data class ScoreboardData(
    val title: String,
    val userTeam: TeamData,
    val opponentTeam: TeamData,
    val rounds: ArrayList<Round>,
    val isCompleted: Boolean
) {
    companion object{

        private val teamLabel = Label(
                en= "Team one",
                fr = "Équipe un"
        )

        private val opponentLabel = Label(
                en= "Team two",
                fr = "Équipe deux"
        )

        fun newInstanceFromScoreboard(title: String,
                                      teamOne: Team,
                                      teamTwo: Team,
                                      rounds: ArrayList<Round>,
                                      isCompleted: Boolean): ScoreboardData {
            val teamData1 = TeamData(
                    teamLabel.getValue(),
                    PlayerData.newInstanceFromPlayer(teamOne.playerOne),
                    PlayerData.newInstanceFromPlayer(teamOne.playerTwo),
                    Labels.SCORE.getValue(),
                    teamOne.score.toString())

            val teamData2 = TeamData(
                    opponentLabel.getValue(),
                    PlayerData.newInstanceFromPlayer(teamTwo.playerOne),
                    PlayerData.newInstanceFromPlayer(teamTwo.playerTwo),
                    Labels.SCORE.getValue(),
                    teamTwo.score.toString())
            
            return ScoreboardData(title, teamData1, teamData2, rounds, isCompleted)
        }
    }
}

class ScoreboardFragment(private val data: ScoreboardData, val isCurrentPlayerWon: Boolean?) : Fragment(){

    companion object{
        fun newInstance(scoreboardData: ScoreboardData, isCurrentPlayerWon: Boolean?): ScoreboardFragment {
            return ScoreboardFragment(scoreboardData, isCurrentPlayerWon)
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.layout_scoreboard, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        view.text_view_scoreboard.text = data.title
        val teamView = view.player_team_card
        val teamCard = buildTeamCard(teamView)
        teamCard.setTeamData(data.userTeam)
        val opponentsTeamView = view.opponents_team_card
        val opponentsTeamCard = buildTeamCard(opponentsTeamView)
        opponentsTeamCard.setTeamData(data.opponentTeam)
        if(data.isCompleted) {
            setViewVisibility(
                view.textView_end_of_game_message,
                view.spinner,
                teamView.text_view_player1_role,
                teamView.text_view_palyer2_role,
                opponentsTeamView.text_view_player1_role,
                opponentsTeamView.text_view_palyer2_role
            )
            if(isCurrentPlayerWon == true)
            {
                celebrate()
            }
            view.textView_state_message.text = Labels.END_MESSAGE(isCurrentPlayerWon)
        }
        else
        {
            view.textView_state_message.text = Labels.ROUND_MESSAGE(data.rounds.size)
        }
    }

    private fun setViewVisibility(vararg views: View) {
        views.forEach {view ->
            view.visibility = View.GONE
        }
    }

    private fun buildTeamCard(teamView: View): TeamCard {
        val teamPlayerOne = PlayerInfo(
            teamView.text_view_player1_name,
            teamView.image_view_player1,
            teamView.text_view_player1_role
        )
        val teamPlayerTwo = PlayerInfo(
            teamView.text_view_player2_name,
            teamView.image_view_player2,
            teamView.text_view_palyer2_role
        )
        return TeamCard(
            teamView.text_view_team_name,
            teamPlayerOne,
            teamPlayerTwo,
            teamView.text_view_label,
            teamView.text_view_team_score
        )
    }
    private fun celebrate() {
        Sound.play(view!!.context, Sounds.VICTORY, 0)
        viewKonfetti.build()
                .addColors(Color.YELLOW, Color.GREEN, Color.MAGENTA)
                .setDirection(0.0, 359.0)
                .setSpeed(1f, 5f)
                .setFadeOutEnabled(true)
                .setTimeToLive(2000L)
                .addShapes(Shape.RECT, Shape.CIRCLE)
                .addSizes(Size(20))
                .setPosition(-50f, viewKonfetti.width + 50f, -50f, +50f)
                .streamFor(300, 5000L)
    }
}