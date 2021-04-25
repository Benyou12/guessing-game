package com.example.polychat.services.messenger

import com.example.polychat.models.socket.*
import com.example.polychat.services.serverServices.ConnectivityManager
import com.example.polychat.services.serverServices.SocketSingleton
import android.content.Context
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.EditText
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.polychat.R
import com.example.polychat.models.Message
import com.example.polychat.views.DialogBox
import com.example.polychat.views.IncomingMessageItem
import com.example.polychat.views.ModeratorMessageItem
import com.example.polychat.views.OutcomingMessageItem
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_chat_log.*

class MessengerService {

    companion object {
        //Conversations
        private val action = "action"
        fun getConversations(uid: String) {
            val userId = Uid(uid)
            val route: String = SocketEvents.GET_CONVERSATIONS.event
            val socketAction = SocketAction(route, userId)
            SocketSingleton.emit(action, socketAction)
        }

        fun getConversation(cid: String) {
            val payload = Cid(cid)
            val route = SocketEvents.GET_CONVERSATION.event
            val socketAction = SocketAction(route, payload)
            SocketSingleton.emit(action, socketAction)
        }

        fun createConversation(uIds: ArrayList<String>, name: String) {
            val route = SocketEvents.CREATE_CONVERSATION.event
            val payload = NewConversation(uIds,name)
            val socketAction = SocketAction(route, payload)
            SocketSingleton.emit(action, socketAction)
        }

        fun joinConversation(uid: String, cid: String) {
            val route = SocketEvents.CONVERSATION_ADD_USER.event
            val payload = AddUserToConvoRequest(cid,uid)
            val socketAction = SocketAction(route,payload)
            SocketSingleton.emit(action,socketAction)
        }

        fun leaveConversation(uid: String, cid: String){
            val route = SocketEvents.CONVERSATION_REMOVE_USER.event
            val payload = AddUserToConvoRequest(cid,uid)
            val socketAction = SocketAction(route,payload)
            SocketSingleton.emit(action,socketAction)
        }

        fun inviteUserToConversation(cid: String, uid: String){
            val route = SocketEvents.INVITE_USER.event
            val payload = AddUserToConvoRequest(cid,uid)
            val socketAction = SocketAction(route,payload)
            SocketSingleton.emit(action,socketAction)
        }

        //Messages
        fun getConversationMessages(cid: String) {
            val route = SocketEvents.GET_CONVERSATION_MESSAGES.event
            val payload = Cid(cid)
            val socketAction = SocketAction(route, payload)
            SocketSingleton.emit(action, socketAction)
        }

        fun emitNewMessage(cid: String, uid: String, text: String){
            val route = SocketEvents.CREATE_MESSAGE.event
            val socketMessage = SocketMessage(text, Uid(uid))
            val newMsgPayload = NewMessagePayload(cid,socketMessage)
            val payload = SocketAction(route,newMsgPayload)
            SocketSingleton.emit(action, payload)
        }

        //Users
        fun getAllUsers(){
            val route = SocketEvents.GAT_ALL_USERS.event
            val socketAction = SocketAction(route, Unit)
            SocketSingleton.emit(action,socketAction)
        }

        fun getOnlineUsers() {
            val route = SocketEvents.GET_USER_ONLINE.event
            val socketAction = SocketAction(route, Unit)
            SocketSingleton.emit(action,socketAction)
        }

        fun getCurrentUserStats(uid: String){
            val route  = SocketEvents.GET_USER_STATS.event
            val socketAction = SocketAction(route, Uid(uid))
            SocketSingleton.emit(action,socketAction)
        }
        fun getConversationUsers(cid: String) {
            val route = SocketEvents.GET_CONVERSATION_USERS.event
            val payload = Cid(cid)
            val socketAction = SocketAction(route, payload)
            SocketSingleton.emit(action, socketAction)
        }

        private fun isMessageValid(str: String): Boolean {
            val msg = str.trim()
            return msg.isNotEmpty()
        }

        private fun checkMessage(ctx: Context, cid: String, uid: String, message: String): MessengerStatus {
            if(!isConnectToNetwork(ctx))return MessengerStatus.IsNotConnected
            if(!isMessageValid(message)) return MessengerStatus.IsInvalid
            emitNewMessage(cid, uid, message)
            return MessengerStatus.Sent
        }

        private fun isConnectToNetwork(ctx: Context): Boolean {
            return ConnectivityManager.isConnectToNetwork(ctx)
        }

        fun sendMessage(ctx: Context, editText: EditText, cid: String, uid: String) {
            val text = editText.text.toString()
            when (checkMessage(ctx, cid, uid, text))
            {
                MessengerStatus.IsInvalid -> editText.text.clear()
                MessengerStatus.IsNotConnected -> {
                    DialogBox.display(
                            context = ctx,
                            title = ctx.resources.getString(R.string.message_box_title),
                            message = ctx.resources.getString(R.string.connection_error_message))
                }
                MessengerStatus.Sent -> editText.text.clear()
            }
        }

        fun addMessageToChatLog(message: Message, uid: String, adapter: GroupAdapter<ViewHolder>, recyclerView: RecyclerView) {
            when {
                message.user.uid == uid -> adapter.add(OutcomingMessageItem(message))
                message.user.uid.contains("moderator") -> adapter.add(ModeratorMessageItem(message))
                else -> adapter.add(IncomingMessageItem(message))
            }
            recyclerView.scrollToPosition(adapter.itemCount - 1)
        }
    }
}

enum class MessengerStatus(val value: String){
    IsNotConnected("Network issue"),
    IsInvalid("Invalid Form"),
    Sent("sent")
}