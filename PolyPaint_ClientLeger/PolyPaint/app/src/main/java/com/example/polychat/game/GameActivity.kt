package com.example.polychat.game

import android.os.Bundle
import android.view.KeyEvent
import android.view.View
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import com.example.polychat.*
import com.example.polychat.base.BaseActivity
import com.example.polychat.framgmentsViews.*
import com.example.polychat.models.*
import com.example.polychat.models.game.*
import com.example.polychat.models.socket.GameUpdate
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.Labels
import com.example.polychat.services.game.GameService
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.Sound
import com.example.polychat.services.util.UIMode
import com.example.polychat.viewModel.ConversationsViewModel
import com.example.polychat.viewModel.GameViewModel
import com.example.polychat.viewModel.MessagesViewModel
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.activity_game.*
import kotlinx.android.synthetic.main.layout_chat_log.*
import kotlinx.android.synthetic.main.menu_item_messenger.view.*
import kotlinx.android.synthetic.main.navbar_game.view.*
import org.json.JSONObject


enum class GameState(val state: Int) {
    LOBBY(0),
    SCOREBOARD(1),
    ActiveGames(2),
    COMPLETED(4),
    DrawerCanvas(5),
    GuesserCanvas(6)
}

class Game : BaseActivity() {

    private var state: GameState = GameState.ActiveGames//GameState.ActiveGames
    private val messagesAdapter = GroupAdapter<ViewHolder>()
    private var generalConversationID: String = "general"
    private lateinit var activeConversation: Conversation
    private val messagesVM = MessagesViewModel()
    private val conversationVM = ConversationsViewModel()
    private val gameVM = GameViewModel()
    lateinit var game: GameData
    private lateinit var currentPlayer: Player
    private var showHistory = false
    private val messagesMap: MutableMap<String,Message> = mutableMapOf()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_game)
        setLanguage(Localization.langCode)
        recycler_view_messages.adapter = messagesAdapter
        onActivityCreated()
        updateView(state.state)
        setButtonEventListener()
        setNavMenu(menu_bar, Activities.GAME)
        setNotificationCounter(menu_bar.msg_item.notifications_counter)
        setOnclickListener()
    }

    override fun onDestroy() {
        super.onDestroy()
        if(::game.isInitialized) {
            GameService.quitQame(game._id,CurrentUser.user.uid)
        }
    }

    private fun setOnclickListener() {
        text_view_history.setOnClickListener {
            showHistory = true
            getConversation(generalConversationID)
        }
    }

    private fun setLanguage(langCode: LangCode) {
        Localization.langCode = langCode
        Localization.setButton(button_send_message, btn_end_of_game)
        Localization.setInput(editText_new_message)
    }

    override fun onKeyUp(keyCode: Int, event: KeyEvent): Boolean {
        return when (keyCode) {
            KeyEvent.KEYCODE_ENTER -> {
                sendMessage()
                true
            }
            else -> super.onKeyUp(keyCode, event)
        }
    }

    private fun setButtonEventListener() {
        button_send_message.setOnClickListener {
            sendMessage()
        }
        btn_end_of_game.setOnClickListener {
            text_view_history.visibility = View.VISIBLE
            setActiveConversation(generalConversationID)
            updateView(GameState.ActiveGames.state)
        }
    }

    private fun setActiveConversation(cid: String) {
        getConversation(cid)
    }

    private fun onActivityCreated() {
        onLogOutReceived()
        onGameUpdated()
        onConversation()
        getConversation(generalConversationID)
        onNewMessage()
        onGameCreated()
        onGameDeleted()
        onGameCanceled()
    }

    private fun onGameCanceled() {
        gameVM.canceledGame.observe(this, Observer {
            //todo to test when a game is canceled
            updateView(GameState.ActiveGames.state)
        })
    }

    private fun onGameDeleted() {
        gameVM.deletedGame.observe(this, Observer {
            // todo when a game deleted
        })
    }

    private fun getConversation(cid: String) {
        MessengerService.getConversation(cid)
    }

    private fun updateView(state: Int) {
        when(state)
        {
            GameState.ActiveGames.state -> {
                btn_end_of_game.visibility = View.GONE
                val games = GameGroups.newInstance(CurrentUser.user.uid)
                displayFragment("activeGames", games)
            }
            GameState.LOBBY.state -> {
                val lobbyData = LobbyData(game._id,game.name, CurrentUser.user,game.name,game.teamOne,game.teamTwo,"Start game")
                val lobby = LobbyFragment.newInstance(lobbyData)
                displayFragment("lobby", lobby)
            }
            GameState.SCOREBOARD.state -> {
                val scoreboard = ScoreboardData.newInstanceFromScoreboard(
                        Labels.SCOREBOARD.getValue(),
                        game.teamOne,
                        game.teamTwo,
                        game.rounds,
                        isCompleted = false)//GameDataMock.scoreboard()
                val teamCard = ScoreboardFragment.newInstance(scoreboard, null)
                displayFragment("teamCard", teamCard)
            }
            GameState.DrawerCanvas.state -> {
                val teams = getTeams(game.teamOne, game.teamTwo)
                val canvas_id = game.rounds.last().canvas._id
                val wordToDraw = game.rounds.last().game_img.word // todo update round model
                val canvas = CanvasFragment.newInstance(canvas_id, teams, wordToDraw)
                displayFragment("canvasFragment", canvas)
            }
            GameState.GuesserCanvas.state -> {
                val teams = getTeams(game.teamOne, game.teamTwo)
                val canvas = GuesserCanvasFragment.newInstance(game, currentPlayer, teams)
                displayFragment("GuesserCanvasFragment", canvas)
            }
            GameState.COMPLETED.state -> {
                val specialScoreboard = ScoreboardData.newInstanceFromScoreboard(
                        Labels.SCOREBOARD.getValue(),
                        game.teamOne,
                        game.teamTwo,
                        game.rounds,
                        isCompleted = true)
                val isCurrentPlayerWon = isWin()
                val teamCard = ScoreboardFragment.newInstance(specialScoreboard, isCurrentPlayerWon)
                btn_end_of_game.visibility = View.VISIBLE
                displayFragment("teamCard", teamCard)
            }
        }
    }

    private fun isWin(): Boolean? {
        val teamWhoWon = getWinnerTeam() ?: return null
        return (teamWhoWon.playerOne!!.user_id == currentPlayer.user_id ||
                teamWhoWon.playerTwo!!.user_id == currentPlayer.user_id)
    }

    private fun getWinnerTeam(): Team? {
        val teamOne = game.teamOne
        val teamTwo = game.teamTwo
        return when
        {
            teamOne.score > teamTwo.score -> teamOne
            teamTwo.score > teamOne.score -> teamTwo
            else -> null
        }
    }

    private fun displayFragment(name: String, fragment: Fragment) {
        clearBackStack()
        val fragmentTransaction = supportFragmentManager.beginTransaction()
        fragmentTransaction.add(R.id.playground, fragment)
        fragmentTransaction.addToBackStack(name)
        fragmentTransaction.commit()
    }

    private fun clearBackStack() {
        if(supportFragmentManager.fragments.size > 0)
            supportFragmentManager.popBackStack()
    }

    // Game's messenger
    private fun setConversationName(name: String) {
        textView_conversation_name.text = name
    }

    private fun onNewMessage() {
        messagesVM.newMessage.observe(this, Observer {
            if(it.message.mid.isEmpty() || it.message.text.isEmpty()) return@Observer
            if(it.cid == activeConversation.cid)
            {
                if(messagesMap.containsKey(it.message.mid)) return@Observer
                messagesMap[it.message.mid] = it.message
                runOnUiThread {
                    Sound.play(this, Sounds.NEW_MESSAGE, 0)
                    addMessageToChatLog(it.message)
                }
            }
            else
            {
                notify(it.cid)
                menu_bar.msg_item.notifications_counter.visibility = View.VISIBLE
            }
        })
    }

    private fun setMessagesMap(messages: ArrayList<Message>?) {
        messagesMap.clear()
        messages?:return
        messages.forEach {
            if (messagesMap.containsKey(it.mid)) return@forEach
            messagesMap[it.mid] = it

        }
    }

    private fun setHistoryVisibility() {
        if(::activeConversation.isInitialized) {
            if(activeConversation.cid != generalConversationID)
                text_view_history.visibility = View.GONE
        }
    }
    
    private fun sendMessage() {
        Sound.play(this, Sounds.SENT, 0)
        MessengerService.sendMessage(this, editText_new_message, activeConversation.cid, CurrentUser.user.uid)
    }

    private fun addMessageToChatLog(message: Message) {
        var messageToDisplay = message
        if(message.user.uid.contains("virtual") || message.user.uid.contains("moderator")) messageToDisplay = localizeMessage(message)
        MessengerService.addMessageToChatLog(messageToDisplay, CurrentUser.user.uid, messagesAdapter, recycler_view_messages)
    }

    private fun localizeMessage(message: Message): Message {
        val text = JSONObject(message.text)
        message.text =  if(Localization.langCode == LangCode.FR) text[LangCode.FR.code].toString() else text[LangCode.EN.code].toString()
        return message
    }

    private fun onGameCreated(){
        gameVM.createdGameData.observe(this, Observer { newGame ->
            game = newGame
            if(CurrentUser.user.uid == game.teamOne.playerOne!!.user_id)
            {
                updateView(GameState.LOBBY.state)
                getConversation(game.conversation_id)
            }
        })
    }

    private fun onConversation(){
        conversationVM.conversation.observe(this, Observer {conversation ->
            activeConversation = conversation
            setMessagesMap(conversation.messages)
            setConversationName(activeConversation.convName!!)
            setHistoryVisibility()
            displayMessages(conversation)
        })
    }

    private fun displayMessages(conversation: Conversation) {
            messagesAdapter.clear()
            if(!showHistory && conversation.cid == generalConversationID) return
            if(conversation.messages == null) return
            runOnUiThread {
                conversation.messages.forEach {message ->
                    addMessageToChatLog(message)
                }
            }
    }

    private fun onGameUpdated(){
        gameVM.updatedGameData.observe(this, Observer { updatedGame->
                game = updatedGame
                if(game.state != GameState.LOBBY.state && updatedGame.updateAction == GameUpdate.USER_QUIT.value)
                {
                    MessengerService.leaveConversation(CurrentUser.user.uid,activeConversation.cid)
                    updateView(GameState.ActiveGames.state)
                    setActiveConversation(generalConversationID)
                    setHistoryVisibility()
                    return@Observer
                }
                val currentState = when {
                    game.state <= GameState.SCOREBOARD.state -> game.state
                    game.state == GameState.COMPLETED.state -> game.state
                    else -> defineState(game)
                }
                updateView(currentState)
                requestConversation()
        })
    }

    private fun requestConversation() {
            if(activeConversation.cid == generalConversationID)
                getConversation(game.conversation_id)
    }

    private fun getTeams(teamOne: Team, teamTwo: Team): Teams {
        val uid = CurrentUser.user.uid
        return if(uid == teamOne.playerOne!!.user_id || uid == teamOne.playerTwo!!.user_id) {
            Teams(teamOne, teamTwo)
        } else {
            Teams(teamTwo, teamOne)
        }
    }

    private fun defineState(g: GameData): Int {
        currentPlayer = getUserPlayer(
                g.teamOne.playerOne,
                g.teamOne.playerTwo,
                g.teamTwo.playerOne,
                g.teamTwo.playerTwo) ?: return -1
        return if(currentPlayer.role == PLAYER_ROLE.DRAW.value){
            GameState.DrawerCanvas.state
        }else {
            GameState.GuesserCanvas.state
        }
    }

    private fun getUserPlayer(vararg players: Player?): Player? {
        for (player in players) {
            if (player!!.user_id == CurrentUser.user.uid) return player
        }
        return null
    }

}