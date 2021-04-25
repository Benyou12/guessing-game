package com.example.polychat.views

import com.example.polychat.R
import com.example.polychat.models.Message
import com.example.polychat.services.serverServices.Time
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_incoming_message.view.*
import kotlinx.android.synthetic.main.layout_moderator.view.*
import kotlinx.android.synthetic.main.layout_outgoing_message.view.*

class IncomingMessageItem(private val message: Message): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_incoming_message
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.text_view_incoming_message.text = message.text
        viewHolder.itemView.text_view_username.text = message.user.username
        viewHolder.itemView.text_view_timestamp.text = Time.getFormattedTime(message.timestamp)
    }
}

class OutcomingMessageItem(private val message: Message): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_outgoing_message
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.outcoming_text_view_username.text = message.user.username
        viewHolder.itemView.outcoming_text_view_timestamp.text = Time.getFormattedTime(message.timestamp)
        viewHolder.itemView.text_view_outcoming_message.text = message.text
    }
}

class ModeratorMessageItem(private val message: Message): Item<ViewHolder>() {
    override fun getLayout(): Int {
        return R.layout.layout_moderator
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.text_view_username_moderator.text= message.user.username
        viewHolder.itemView.text_view_timestamp_moderator.text = Time.getFormattedTime(message.timestamp)
        viewHolder.itemView.text_view_incoming_message_moderator.text = message.text
    }
}
