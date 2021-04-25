package com.example.polychat.views

import com.example.polychat.R
import com.example.polychat.models.userModels.UserStats
import com.squareup.picasso.Picasso
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_user_row.view.*

class UserItem(val user: UserStats): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_user_row
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.u_userName.text = user.username
        Picasso.get().load(user.profileImgUrl).into(viewHolder.itemView.u_profileImg)
    }
}