package com.example.polychat.messenger
import com.example.polychat.views.ConversationSearchAdapter
import android.os.Bundle
import android.util.Log
import android.view.KeyEvent
import android.view.View
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.models.*
import com.example.polychat.models.socket.*
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.Notification
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.serverServices.SocketSingleton
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.Sound
import com.example.polychat.services.util.UIMode
import com.example.polychat.viewModel.ConversationsViewModel
import com.example.polychat.viewModel.MessagesViewModel
import com.example.polychat.viewModel.UserViewModel
import com.example.polychat.views.*
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.activity_messenger.*
import kotlinx.android.synthetic.main.layout_chat_log.*
import kotlinx.android.synthetic.main.layout_conversations.*
import kotlinx.android.synthetic.main.menu_item_messenger.view.*
import kotlinx.android.synthetic.main.navbar_game.view.*
import org.json.JSONObject

class MessengerActivity : BaseActivity() {

    companion object {
        const val TAG: String = "#home"
    }

    private lateinit var activeConversation: Conversation
    private val chatLogAdapter = GroupAdapter<ViewHolder>()
    private val conversationsAdapter = GroupAdapter<ViewHolder>()
    private val newMessageVM = MessagesViewModel()
    private val conversationsVM = ConversationsViewModel()
    private val messagesVM = MessagesViewModel()
    private val userVM = UserViewModel()
    private val generalConversationId = "general"
    private var request = Request.NONE
    private var conversationsByCID: MutableMap<String,ConversationHistory> = mutableMapOf()
    private var firstReceivedMessageTimestamp: Long = Long.MAX_VALUE
    private var isFirstMessage: Boolean = true
    private var usersToAdd: ArrayList<String> = arrayListOf()
    private var onlineUsersMap: MutableMap<String,User> = mutableMapOf()
    private var messagesMap: MutableMap<String,Message> = mutableMapOf()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_messenger)
        setLanguage(Localization.langCode)
        Log.d(TAG, CurrentUser.user.username)
        recycler_view_messages.adapter = chatLogAdapter
        recycler_view_messages.adapter = chatLogAdapter
        recycler_view_conversations.adapter = conversationsAdapter
        onCreated()
        setNavMenu(menu_bar_messenger, Activities.MESSENGER)
        button_send_message.setOnClickListener {
              sendMessage(activeConversation.cid)
        }

        floatingActionButton_add_conversation.setOnClickListener {
            createConversation()
        }

        edit_text_search_conversation.setOnClickListener {
            requestAllConversations()
        }

        text_view_invite.setOnClickListener{
            showAutoCompleteView()
        }

        auto_complete_invitation.setOnClickListener {
            requestOnlineUsers()
        }

        text_view_leave.setOnClickListener {
            leaveConversation()
        }

        text_view_history.setOnClickListener {
            displayConversationHistory()
        }
    }

    private fun createConversation() {
        usersToAdd.add(CurrentUser.user.uid)
        DialogBox.createNewConversation(this, usersToAdd)
    }

    private fun displayConversationHistory() {
        request = Request.HISTORY
        conversationsByCID[activeConversation.cid]!!.showHistory = true
        getConversationMessages(activeConversation.cid)
    }

    private fun leaveConversation() {
        request = Request.LEAVE
        DialogBox.leaveConversation(this,CurrentUser.user.uid,activeConversation)
    }

    private fun requestOnlineUsers() {
        MessengerService.getOnlineUsers()
    }

    private fun showAutoCompleteView() {
        val visibility = auto_complete_invitation.visibility
        auto_complete_invitation.visibility  = if (visibility == View.GONE) View.VISIBLE else View.GONE
    }

    private fun setLanguage(langCode: LangCode) {
        Localization.langCode = langCode
        Localization.setButton(button_send_message)
        Localization.setInput(editText_new_message)
        Localization.setAutoCompleteTextView(edit_text_search_conversation,auto_complete_invitation)
        Localization.setTextView(text_view_leave,text_view_invite,text_view_history)
    }

    private fun requestAllConversations() {
        val route = SocketEvents.SEARCH_REQUEST.event
        val socketAction = SocketAction(route, Unit)
        SocketSingleton.emit("action", socketAction)
    }
    private fun onSearchConversation() {
        conversationsVM.searchConversationsResponse.observe(this, Observer {
            Log.d("#SCS", "$it")
            val conversations = it.toList()
            showSuggestions(conversations)
            onSearConversationSelected()
        })
    }

    private fun onOnlineUsersReceived() {
        userVM.onlineUsers.observe(this, Observer {onlineUsers ->
            setOnlineUsersMap(onlineUsers)
            showUsersList()
            onUserSelected()
        })
    }

    private fun setOnlineUsersMap(onlineUsers: Array<User>) {
        onlineUsersMap = mutableMapOf()
        onlineUsers.forEach{ user ->
            if(!activeConversation.uids.contains(user.uid))
                onlineUsersMap[user.uid] = user
        }
    }

    private fun showSuggestions(conversations: List<Conversation>) {
        val adapter = ConversationSearchAdapter(this, android.R.layout.simple_list_item_1, conversations)
        edit_text_search_conversation.setAdapter(adapter)
        edit_text_search_conversation.threshold = 1
    }

    private fun onSearConversationSelected() {
        edit_text_search_conversation.setOnItemClickListener { parent, _, position, _ ->
            val selectedConversation = parent.adapter.getItem(position) as Conversation?
            displaySelectedConversation(selectedConversation!!)
            edit_text_search_conversation.text.clear()
        }
    }

    private fun displaySelectedConversation(selectedConversation: Conversation) {
        if (!selectedConversation.uids.contains(CurrentUser.user.uid))
            askUserToJoin(CurrentUser.user.uid,selectedConversation)
        else {
            activeConversation = selectedConversation
            setActiveConversationLabel(activeConversation.convName)
            getConversationMessages(selectedConversation.cid)
            request = Request.NONE
        }
    }

    private fun askUserToJoin(uid: String, conversation: Conversation) {
        DialogBox.askUserToJoin(this,uid,conversation)
        request = Request.JOIN
    }

    private fun onConversationJoined(){
        conversationsVM.conversationUpdated.observe(this, Observer {conversation ->
            when(conversation.updateAction) {
                CONVERSATION_UPDATE.USER_ADDED.value -> addUserToConversation(conversation)
                CONVERSATION_UPDATE.USER_REMOVED.value -> removeUserFromConversation(conversation)
            }
        })
    }

    private fun addUserToConversation(conversation: Conversation) {
        if(request == Request.JOIN) {
            joinConversation(conversation)
        }
        updateConversationUIds(conversation)
    }

    private fun removeUserFromConversation(conversation: Conversation) {
        updateConversationUIds(conversation)
        if(request == Request.LEAVE) {
            removeConversation(conversation)
        }
    }

    private fun updateConversationUIds(conversation: Conversation){
        conversationsByCID[conversation.cid] ?: return
        conversationsByCID[conversation.cid]!!.conversation.uids = conversation.uids
    }

    private fun joinConversation(conversation: Conversation) {
        conversationsByCID[conversation.cid]= ConversationHistory(conversation,false)
        setActiveConversation(conversation)
        displayConversations()
        setActiveConversationLabel(activeConversation.convName)
        setViewsVisibility()
        request = Request.NONE
        getConversationMessages(activeConversation.cid)
    }

    private fun removeConversation(conversation: Conversation) {
            //todo localization should be done inside DialogBox class
            val titleFieldName = "leave_conversation_dialog_box_title"
            val messageFieldName = "leave_conversation_dialog_box_message"
            val locale = Localization.locaLizedAlertDialog(titleFieldName,messageFieldName)
            conversationsByCID.remove(conversation.cid)
            DialogBox.display(
                context = this,
                title = locale[titleFieldName].toString(),
                message = locale[messageFieldName].toString()
            )
            setActiveConversation(conversationsByCID[generalConversationId]!!.conversation)
            setViewsVisibility()
            displayConversations()
            setActiveConversationLabel(activeConversation.convName)
            getConversationMessages(activeConversation.cid)
            request = Request.NONE
    }

    override fun onKeyUp(keyCode: Int, event: KeyEvent): Boolean {
        return when (keyCode) {
            KeyEvent.KEYCODE_ENTER -> {
                sendMessage(activeConversation.cid)
                true
            }
            else -> super.onKeyUp(keyCode, event)
        }
    }

    private fun onCreated() {
        frame_conversations.alpha = 0f
        onLogOutReceived()
        getConversations()
        onConversations()
        onNewMessage()
        onConversation()
        onNewConversation()
        onMessages()
        onConversationClick()
        onSearchConversation()
        onConversationJoined()
        listenToSocketErrors()
        onOnlineUsersReceived()
        onInviteUserResponse()
    }

    private fun showUsersList() {
        val adapter = UserSearchAdapter(this, android.R.layout.simple_list_item_1,onlineUsersMap.values.toList())
        auto_complete_invitation.setAdapter(adapter)
        auto_complete_invitation.threshold = 1
    }

    private fun onUserSelected() {
        auto_complete_invitation.setOnItemClickListener { parent, _, position, _ ->
            val selectedUser = parent.adapter.getItem(position) as User
            auto_complete_invitation.text.clear()
            auto_complete_invitation.visibility = View.GONE
            confirmInvitation(selectedUser)
        }
    }

    private fun confirmInvitation(selectedUser: User) {
            DialogBox.inviteUserToConversation(this, activeConversation,selectedUser)
    }

    private fun onNewMessage() {
        newMessageVM.newMessage.observe(this, Observer {
            if(isFirstMessage) {
                firstReceivedMessageTimestamp = it.message.timestamp
                isFirstMessage = false
            }
            if (it.cid == activeConversation.cid) {
                if(messagesMap.containsKey(it.message.mid)) return@Observer
                runOnUiThread {
                    if(it.message.user.uid != CurrentUser.user.uid) Sound.play(this, Sounds.NEW_MESSAGE, 0)
                    messagesMap[it.message.mid] = it.message
                    addMessageToChatLog(it.message)
                }
            }
            else
            {
                notify(it.cid)
                if(convoItems.containsKey(it.cid))
                {
                    val convo = convoItems[it.cid]
                    putToTop(convo!!)
                }
            }
        })
    }

    private fun putToTop(convo: ConversationItem) {
        conversationsAdapter.remove(convo)
        conversationsAdapter.add(0, convo)
        conversationsAdapter.notifyDataSetChanged()
    }

    private fun onConversations() {
        conversationsVM.conversations.observe(this, Observer {conversations->
            runOnUiThread {
                setConversationsMap(conversations)
                setViewsVisibility()
                displayConversations()
                request = Request.NONE
                setActiveConversation(conversationsByCID[activeConversation.cid]!!.conversation)
                setActiveConversationLabel(activeConversation.convName)
                getConversationMessages(activeConversation.cid)
                frame_conversations.alpha = 1f
                conv_spinner.visibility = View.GONE
            }
        })
    }

    private fun setActiveConversation(conversation: Conversation) {
        activeConversation = conversation
    }

    private fun setConversationsMap(conversations: Array<Conversation>) {
        conversations.forEach { conversation ->
            conversationsByCID[conversation.cid] = ConversationHistory(conversation,false)
        }
    }

    private fun onConversation(){
        conversationsVM.conversation.observe(this, Observer {conversation ->
            getConversationMessages(conversation.cid)
        })
    }

    private fun onMessages() {
        messagesVM.messages.observe(this, Observer { messages ->
            runOnUiThread {
                addMessagesToChatLog(messages)
            }
        })
    }

    private fun onNewConversation(){
        conversationsVM.newConversation.observe(this, Observer {conversation->
            conversationsByCID[conversation.cid] = ConversationHistory(conversation,false)
            runOnUiThread {
                displayConversation(conversation)
            }
        })
    }

    //Conversations
    private fun displayConversations() {
        conversationsAdapter.clear()
        conversationsByCID.forEach { (_, conversationHistory) ->
            if(!::activeConversation.isInitialized) activeConversation = conversationHistory.conversation
            displayConversation(conversationHistory.conversation)
        }
    }

    var convoItems: MutableMap<String, ConversationItem> = mutableMapOf()
    private fun displayConversation(conversation: Conversation) {
        val convoItem = ConversationItem(conversation)
        convoItems.putIfAbsent(convoItem.conversation.cid, convoItem)
        conversationsAdapter.add(0, convoItem)
        conversationsAdapter.notifyDataSetChanged()
        convoItems.putIfAbsent(conversation.cid, convoItem)
    }

    private fun getConversations() {
        MessengerService.getConversations(CurrentUser.user.uid)
    }

    private fun getConversationMessages(cid: String) {
        MessengerService.getConversationMessages(cid)
    }

    private fun onConversationClick(){
        conversationsAdapter.setOnItemClickListener { item, _ ->
            val conversationItem = item as ConversationItem
            conversationItem.deleteNotificationBadge()
            val cid = conversationItem.conversation.cid
            if(Notification.unreadConvosId.contains(cid))
            {
                Notification.unreadConvosId.remove(cid)
                setNotificationCounter(menu_bar_messenger.msg_item.notifications_counter)
            }
            activeConversation = conversationItem.conversation
            chatLogAdapter.clear()
            setViewsVisibility()
            setActiveConversationLabel(activeConversation.convName)
            request = Request.NONE
            getConversationMessages(activeConversation.cid)
        }
    }

    private fun onInviteUserResponse(){
        conversationsVM.inviteUserResponse.observe(this, Observer {response ->
            if(response.uid == CurrentUser.user.uid)
            {
                acceptInvitation(response)
                request = Request.JOIN
            }

        })
    }

    private fun acceptInvitation(resp: InviteUserResponse) {
        DialogBox.acceptInvitation(this,resp)

    }

    private fun setActiveConversationLabel(name: String?) {
        textView_conversation_name.text = name
    }

    //Messages
    private fun addMessagesToChatLog(messages: Array<Message>?) {
        if (messages == null) return
        chatLogAdapter.clear()
        messagesMap.clear()
        messages.forEach {message->
            messagesMap[message.mid] = message
            if(::activeConversation.isInitialized) {
                if (conversationsByCID[activeConversation.cid]!!.showHistory || message.timestamp >= firstReceivedMessageTimestamp)
                    addMessageToChatLog(message)
            }
        }
    }


    private fun localizeMessage(message: Message): Message {
        val text = JSONObject(message.text)
        message.text =  if(Localization.langCode == LangCode.FR) text[LangCode.FR.code].toString() else text[LangCode.EN.code].toString()
        return message
    }
    private fun addMessageToChatLog(message: Message) {
        var messageToDisplay = message
        if(message.user.uid.contains("virtual") || message.user.uid.contains("moderator")) messageToDisplay = localizeMessage(message)
        MessengerService.addMessageToChatLog(messageToDisplay, CurrentUser.user.uid, chatLogAdapter, recycler_view_messages)
    }

    private fun sendMessage(cid: String) {
        Sound.play(this, Sounds.SENT, 0)
        MessengerService.sendMessage(this, editText_new_message, cid, CurrentUser.user.uid)
    }

    private fun setViewsVisibility() {
        if(::activeConversation.isInitialized) {
            if (activeConversation.cid == generalConversationId){
                text_view_leave.visibility = View.GONE
                text_view_invite.visibility = View.GONE
                auto_complete_invitation.visibility = View.GONE
            } else {
                text_view_leave.visibility = View.VISIBLE
                text_view_invite.visibility = View.VISIBLE
            }
        }
    }

    private fun listenToSocketErrors(){
        SocketSingleton.socket.on("error"){
            val error = it[0].toString()
            Log.d(TAG, error)
            //todo to remove before release - used now to display socket errors sent by server
//            runOnUiThread {
//                DialogBox.display(
//                    context = this,
//                    title = "Error du serveur",
//                    message = error
//                )
//            }
        }
    }
}

enum class Request{
    LEAVE,
    JOIN,
    HISTORY,
    NONE
}
data class ConversationHistory(var conversation: Conversation, var showHistory: Boolean )

