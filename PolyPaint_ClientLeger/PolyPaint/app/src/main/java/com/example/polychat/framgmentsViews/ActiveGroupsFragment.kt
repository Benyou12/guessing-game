package com.example.polychat.framgmentsViews

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.models.game.GameData
import com.example.polychat.models.game.Player
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.game.LobbyService
import com.example.polychat.services.util.Localization
import com.example.polychat.viewModel.GameViewModel
import com.squareup.picasso.Picasso
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import de.hdodenhof.circleimageview.CircleImageView
import kotlinx.android.synthetic.main.layout_active_groups.*
import kotlinx.android.synthetic.main.layout_active_groups.view.*
import kotlinx.android.synthetic.main.layout_group.view.*

class GroupItem(private val game: GameData, private val name: String, private val event: View.OnClickListener): Item<ViewHolder>() {

    override fun getLayout(): Int {
        return R.layout.layout_group
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        val view = viewHolder.itemView
        view.text_view_group_name.text = name
        view.button_join_group.setOnClickListener(event)
        setLanguage(view.button_join_group)
        displayProfileImages(view)
    }

    private fun displayProfileImages(view: View)
    {
        val presentPlayers = getPresentPlayers(
            game.teamOne.playerOne,
            game.teamOne.playerTwo,
            game.teamTwo.playerOne,
            game.teamTwo.playerTwo)
        displayPresentPlayersImages(presentPlayers,view)
    }

    private fun displayPresentPlayersImages(
        presentPlayers: ArrayList<Player>,
        view: View
    ) {
        when(presentPlayers.size) {
            1 ->
            {
                displayImage(presentPlayers[0],view.image_view_group_player_one)
                view.image_view_group_player_two.visibility = View.GONE
                view.image_view_group_player_three.visibility = View.GONE
                view.image_view_group_player_four.visibility = View.GONE
            }
            2 ->
            {
                displayImage(presentPlayers[0],view.image_view_group_player_one)
                displayImage(presentPlayers[1],view.image_view_group_player_two)
                view.image_view_group_player_three.visibility = View.GONE
                view.image_view_group_player_four.visibility = View.GONE
            }
            3 ->
            {
                displayImage(presentPlayers[0],view.image_view_group_player_one)
                displayImage(presentPlayers[1],view.image_view_group_player_two)
                displayImage(presentPlayers[2],view.image_view_group_player_three)
                view.image_view_group_player_four.visibility = View.GONE
            }
            4 ->
            {
                displayImage(presentPlayers[0],view.image_view_group_player_one)
                displayImage(presentPlayers[1],view.image_view_group_player_two)
                displayImage(presentPlayers[2],view.image_view_group_player_three)
                displayImage(presentPlayers[3],view.image_view_group_player_four)
            }
        }

    }

    private fun getPresentPlayers(vararg players: Player?): ArrayList<Player> {
        val presentPlayers = ArrayList<Player>()
        for( player in  players)
        {
            player ?: continue
            presentPlayers.add(player)
        }

        return presentPlayers
    }

    private fun displayImage(player: Player, profileImage: CircleImageView) {
        profileImage.visibility = View.VISIBLE
        setImageUrl(player.user.profileImgUrl, profileImage)
    }

    private fun setLanguage(buttonJoinGroup: Button) {
        Localization.setButton(buttonJoinGroup)
    }

    private fun setImageUrl(url: String, imageView: ImageView) {
            Picasso.get().load(url).into(imageView)
    }
}

fun joinClickEvent(gameId: String, uid : String): View.OnClickListener {
    return View.OnClickListener {
        LobbyService.joinGame(gameId, uid)
    }
}

class GameGroups(private val uid: String) : Fragment(){
    private val adapter = GroupAdapter<ViewHolder>()
    private val gameVM = GameViewModel()
    private var data: Array<GameData>? = null
    companion object{
        fun newInstance(uid: String): GameGroups {
            return GameGroups(uid)
        }
    }
 override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
    ): View? {
     return inflater.inflate(R.layout.layout_active_groups, container, false)
    }
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val v = view.linear_layout_recycler_view
        v.recycler_view_active_groups.adapter = adapter
        onGameCreated()
        setCreateGameButtonAction(uid)
        setLanguage(v)
        onAllGamesReceived()
        getActiveGames()
    }

    private fun onGameCreated() {
        gameVM.createdGameData.observe(this, Observer {
            if(CurrentUser.user.uid != it.teamOne.playerOne!!.user_id)
            {
                adapter.add(
                    0,
                        GroupItem(
                                it,
                                it.name,
                                joinClickEvent(it._id,CurrentUser.user.uid)))
                adapter.notifyDataSetChanged()
            }
        })

    }

    private fun displayGroups(uid: String) {
        for (item in data!!)
        {
            adapter.add(GroupItem(item, item.name, joinClickEvent(item._id,uid)))
        }
    }

    private fun setLanguage(view: LinearLayout) {
        Localization.setButton(view.button_add_group)
        Localization.setTextView(view.text_view_active_groups)
    }

    private fun setCreateGameButtonAction(uid: String) {
        button_add_group.setOnClickListener {
            LobbyService.createNewGame(uid)
        }
    }

    private fun onAllGamesReceived() {
        gameVM.activesGameData.observe(this, Observer {activeGames ->
            data = activeGames
            displayGroups(uid)
        })
    }

    private fun getActiveGames() {
        LobbyService.getActiveGames()
    }
}