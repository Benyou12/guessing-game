package com.example.polychat.views

import com.example.polychat.R
import com.example.polychat.models.Conversation
import com.example.polychat.models.Label
import com.example.polychat.services.Notification
import com.example.polychat.services.serverServices.Time
import com.squareup.picasso.Picasso
import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_conversation.view.*

val NEW_MESSAGE = Label(fr = "Nouveau", en = "New")

class ConversationItem(val conversation: Conversation): Item<ViewHolder>() {

    private var view: ViewHolder? = null

    override fun getLayout(): Int {
        return R.layout.layout_conversation
    }

    override fun bind(viewHolder: ViewHolder, position: Int) {
        view = viewHolder
        viewHolder.itemView.text_view_conversation_name.text = getDisplayedName(conversation.convName)
        viewHolder.itemView.text_view_conversation_timestamp.text = Time.getFormattedTime(conversation.timestamp)
        setNotificationIndicator()
        Picasso.get().load(conversation.users?.first()?.profileImgUrl).into(viewHolder.itemView.convo_img)
    }

    fun setNotificationIndicator() {
        if(view == null) return
        if(Notification.unreadConvosId.contains(conversation.cid))
        {
            view!!.itemView.text_view_new_message.text = NEW_MESSAGE.getValue()
        } else {
            view!!.itemView.text_view_new_message.text = ""
        }
    }

    fun deleteNotificationBadge() {
       if(view != null)
       {
           view!!.itemView.text_view_new_message.text = ""
       }
    }

    private fun getDisplayedName(name: String?):  String{
        val conversationName: String
        val length = 15
        if (name!!.length > length) {
            conversationName = name.substring(0,length)
            return "$conversationName..."
        }
        return name
    }
}