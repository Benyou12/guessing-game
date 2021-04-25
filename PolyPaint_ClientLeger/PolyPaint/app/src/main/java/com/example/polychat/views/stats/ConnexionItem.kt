package com.example.polychat.views.stats

import android.widget.TextView
import com.example.polychat.R
import com.example.polychat.models.userModels.UserAuthStats
import com.example.polychat.services.Labels
import com.example.polychat.services.serverServices.Time
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_connexion_history_item.view.*

class ConnexionItem(private val userAuthStats: UserAuthStats): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_connexion_history_item
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        val itemView = viewHolder.itemView
        itemView.text_view_connexion_timestamp.text = Time.getFormattedTime(userAuthStats.timestamp)
        setConnexionLabel(itemView.text_view_connexion)
    }

    private fun setConnexionLabel(view: TextView) {
        val connectionType = if(userAuthStats.isLogin) Labels.LOGED_IN.getValue()
                                   else Labels.LOGED_OUT.getValue()
        view.text_view_connexion.text = connectionType
    }

}
