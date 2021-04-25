package com.example.polychat.views.stats

import android.widget.ImageView
import android.widget.TextView
import com.example.polychat.R
import com.example.polychat.models.userModels.UserBadge
import com.example.polychat.services.serverServices.Time
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.squareup.picasso.Picasso
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import de.hdodenhof.circleimageview.CircleImageView
import kotlinx.android.synthetic.main.layout_badge_item.view.*

class BadgeItem(private val userBadge: UserBadge): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_badge_item
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        val itemView = viewHolder.itemView
        displayBadgeName(itemView.text_view_badge_name)
        displayBadge(itemView.img_view_badge)
        itemView.text_view_badge_timestamp.text = Time.getFormattedTime(userBadge.timestamp)
        itemView.text_view_badge_points.text = userBadge.badge.points.toString()
    }

    private fun displayBadge(imgViewBadge: ImageView) {
        val badgeUrl = if (Localization.langCode == LangCode.EN) userBadge.badge.img_en
        else userBadge.badge.img_fr
        Picasso.get().load(badgeUrl).into(imgViewBadge.img_view_badge)
    }

    private fun displayBadgeName(textView: TextView){
        val badgeName = if (Localization.langCode == LangCode.EN) userBadge.badge.name_en
        else userBadge.badge.name_fr
        textView.text = badgeName
    }
}
