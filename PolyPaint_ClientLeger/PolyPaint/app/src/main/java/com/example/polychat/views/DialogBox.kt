package com.example.polychat.views

import android.app.AlertDialog
import android.content.Context
import android.widget.EditText
import com.example.polychat.R
import com.example.polychat.models.Conversation
import com.example.polychat.models.User
import com.example.polychat.models.socket.InviteUserResponse
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.util.Localization

class DialogBox  {

    companion object {
        fun display(context: Context, title: String, message: String){
            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(title)
                setMessage(message)
                setNegativeButton(R.string.negative_button_text){ _, _ ->
                    // Add action to perform when the button is clicked
                }
                create()
                show()
            }
        }

        fun createNewConversation(context: Context,uids: ArrayList<String>){
            val titleFieldName =  "AlertBox_New_Conversation_Title"
            val createBtnFieldName = "AlertBox_New_Conversation_Create"
            val cancelBtnFieldName = "AlertBox_New_Conversation_Cancel"
            val locale = Localization.locaLizedAlertDialog(titleFieldName,createBtnFieldName,cancelBtnFieldName)
            val editTextNewConversation: EditText = EditText(context)
                .apply {
                    this.background = resources.getDrawable(R.drawable.rounded_input_p)
                    this.tag = "editTextNewConversation"
                }
            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(locale[titleFieldName].toString())
                setMessage("")
                setView(editTextNewConversation)
                setPositiveButton(locale[createBtnFieldName].toString()){ _, _ ->
                    val conversationName = editTextNewConversation.text.toString()
                    MessengerService.createConversation(uids,conversationName)
                }
                setNegativeButton(locale[cancelBtnFieldName].toString()){ _, _ -> }
                create()
                Localization.setInput(editTextNewConversation)
                show()
            }
        }

        fun askUserToJoin(context: Context,uid: String, conversation: Conversation) {
            val titleFieldName =  "AlertBox_Join_Conversation_Title"
            val yesButton= "Yes_Button"
            val noButton= "No_Button"
            val message = "AlertBox_Join_Conversation_Message"
            val locale = Localization.locaLizedAlertDialog(titleFieldName,yesButton,noButton,message)

            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(locale[titleFieldName].toString() + conversation.convName)
                setMessage(locale[message].toString())
                setPositiveButton(locale[yesButton].toString()){ _, _ ->
                   MessengerService.joinConversation(uid,conversation.cid)
                }
                setNegativeButton(locale[noButton].toString()){ _, _ -> }
                create()
                show()
            }
        }

        fun inviteUserToConversation(context: Context, conversation: Conversation, user: User){
            val titleFieldName =  "AlertBox_Invite_Conversation_Title"
            val inviteButton= "Invite_Button"
            val cancelBtnFieldName = "AlertBox_New_Conversation_Cancel"
            val messageFieldName = "AlertBox_Invite_Message"
            val hint = "AlertBox_Invite_Hint"
            val locale = Localization.locaLizedAlertDialog(titleFieldName,inviteButton,cancelBtnFieldName,hint,messageFieldName)

            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(locale[titleFieldName].toString() + conversation.convName)
                setMessage(locale[messageFieldName] + user.username + " ?")
                setPositiveButton(locale[inviteButton].toString()){ _, _ ->
                    // todo send invitation once server implements invitation routes
                    MessengerService.inviteUserToConversation(conversation.cid,user.uid)
                }
                setNegativeButton(locale[cancelBtnFieldName].toString()){ _, _ -> }
                create()
                show()
            }
        }

        fun leaveConversation(context: Context,uid: String, conversation: Conversation){
            val titleFieldName =  "AlertBox_Leave_Conversation_Title"
            val yesButton= "Yes_Button"
            val noButton= "No_Button"
            val message = "AlertBox_Leave_Conversation_Message"
            val locale = Localization.locaLizedAlertDialog(titleFieldName,yesButton,noButton,message)

            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(locale[titleFieldName].toString() + conversation.convName)
                setMessage(locale[message].toString())
                setPositiveButton(locale[yesButton].toString()){ _, _ ->
                    MessengerService.leaveConversation(uid,conversation.cid)
                }
                setNegativeButton(locale[noButton].toString()){ _, _ -> }
                create()
                show()
            }
        }

        fun acceptInvitation(context: Context, serverResponse: InviteUserResponse) {
            val titleFieldName = "AlertBox_Accept_Conversation_Title"
            val invitedBy = "AlertBox_Accept_Conversation_By"
            val yesButton= "Yes_Button"
            val noButton= "No_Button"
            val messageFieldName = "AlertBox_Accept_Conversation_message"
            val locale = Localization.locaLizedAlertDialog(
                titleFieldName,
                yesButton,
                noButton,
                invitedBy,
                messageFieldName
            )

            val builder = AlertDialog.Builder(context)
            builder.apply {
                setCancelable(false)
                setTitle(locale[titleFieldName].toString()
                        +serverResponse.conversation.convName
                        +locale[invitedBy]
                        +serverResponse.user.username)
                setMessage(locale[messageFieldName])
                setPositiveButton(locale[yesButton].toString()) { _, _ ->
                    MessengerService.joinConversation(serverResponse.uid,serverResponse.conversation.cid)
                }
                setNegativeButton(locale[noButton].toString()) { _, _ -> }
                create()
                show()
            }
        }
    }
}
