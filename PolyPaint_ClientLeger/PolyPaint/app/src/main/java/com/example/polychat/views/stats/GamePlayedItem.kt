package com.example.polychat.views.stats

import com.example.polychat.R
import com.example.polychat.models.Message
import com.example.polychat.models.userModels.UserGameStatsHistory
import com.example.polychat.services.serverServices.Time
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_game_played_item.view.*

class GamePlayedItem(private val gameHistory: UserGameStatsHistory): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_game_played_item
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.text_view_game_played.text= gameHistory.game_id
        viewHolder.itemView.text_view_game_played_timestamp.text = Time.getFormattedTime(gameHistory.timestamp)
        viewHolder.itemView.text_view_game_played_points.text = gameHistory.myTeamResult.toString()

    }
}
