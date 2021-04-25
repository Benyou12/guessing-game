package com.example.polychat.base
import android.content.Context
import android.content.Intent
import android.graphics.Color
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import com.example.polychat.authentication.SignIn
import com.example.polychat.authentication.SignUp
import com.example.polychat.game.Game
import com.example.polychat.messenger.MessengerActivity
import com.example.polychat.models.Activities
import com.example.polychat.models.Sounds
import com.example.polychat.models.UserOAuth
import com.example.polychat.models.socket.Uid
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.Labels
import com.example.polychat.services.Notification
import com.example.polychat.services.Server
import com.example.polychat.services.serverServices.SocketSingleton
import com.example.polychat.services.util.Sound
import com.example.polychat.settings.PublicProfile
import com.example.polychat.settings.UserProfile
import com.example.polychat.tutorial.TutorialActivity
import com.example.polychat.viewModel.MessagesViewModel
import com.example.polychat.viewModel.UserViewModel
import com.example.polychat.views.DialogBox
import com.google.gson.GsonBuilder
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.menu_item_messenger.view.*
import kotlinx.android.synthetic.main.navbar_game.view.*
import okhttp3.Call
import okhttp3.Callback
import okhttp3.Response
import java.io.IOException

open class BaseActivity : AppCompatActivity() {

    private val userVM = UserViewModel()
    override fun onBackPressed() {
        val title = Labels.BACK_BUTTON_DIALOG_TITLE.getValue()
        val message = Labels.BACK_BUTTON_DIALOG_MSG_.getValue()
        DialogBox.display(
            this,
            title,
            message
        )
    }

    protected fun navigateToSignUpActivity() {
    val intent = Intent(this, SignUp::class.java)
     // added Extra to know is user Signing up with his email or via other providers
    val userOAuth = UserOAuth(null,false)
    val userKey = "user"
    intent.putExtra(userKey, userOAuth)
    intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
    startActivity(intent)
    }

    protected fun navigateToSignInActivity() {
        val intent = Intent(this, SignIn::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToTutorial() {
        val intent = Intent(this, TutorialActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToHomeActivity() {
        val intent = Intent(this, HomeActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToGame() {
        val intent = Intent(this, Game::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToUserProfile() {
        val intent = Intent(this, UserProfile::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToSignIn() {
        val intent = Intent(this, SignIn::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToUsersPage() {
        val intent = Intent(this, PublicProfile::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun navigateToMessenger() {
        val intent = Intent(this, MessengerActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    protected fun loadUrlIntoView(url: String, imgView: ImageView) {
        Picasso.get().load(url).into(imgView)
    }

    protected fun onLogOutReceived() {
        userVM.logOut.observe(this, Observer {
            if (it.uid == CurrentUser.user.uid) {
                SocketSingleton.disconnect()
                val msg = Labels.LOG_OUT_MESSAGE.getValue()
                Toast.makeText(this, msg, Toast.LENGTH_LONG).show()
                navigateToSignIn()
            }
        })
    }

    protected fun setNavMenu(menuBar: View, activity: Activities) {
        // home Item
        val homeItem = menuBar.home_item
        setMenuItem(homeItem, View.GONE, Labels.NAV_BAR_HOME.getValue())
        if(activity == Activities.HOME) setSelectedItemStyle(homeItem)
        else homeItem.setOnClickListener { navigateToHomeActivity() }
        // messenger Item
        val msgItem = menuBar.msg_item
        setMenuItem(msgItem, View.VISIBLE, Labels.NAV_BAR_MESSENGER.getValue())
        if(activity == Activities.MESSENGER) setSelectedItemStyle(msgItem)
        else msgItem.setOnClickListener { navigateToMessenger() }
        // game Item
        val gameItem = menuBar.game_item
        setMenuItem(gameItem, View.GONE, Labels.NAV_BAR_GAME.getValue())
        if(activity == Activities.GAME) setSelectedItemStyle(gameItem)
        else gameItem.setOnClickListener { navigateToGame() }
        // users Item
        val usersItem = menuBar.users_item
        setMenuItem(usersItem, View.GONE, Labels.NAV_BAR_USERS.getValue())
        if(activity == Activities.USERS) setSelectedItemStyle(usersItem)
        else usersItem.setOnClickListener { navigateToUsersPage() }
        // SignOut Item
        val signOutItem = menuBar.signOut_item
        setMenuItem(signOutItem, View.GONE, Labels.NAV_BAR_SIGN_OUT.getValue())
        if(activity == Activities.SIGN_OUT) setSelectedItemStyle(signOutItem)
        else signOutItem.setOnClickListener { signOut(this) }
        // user Item
        val userItem = menuBar.user_profile
        loadUrlIntoView(CurrentUser.user.profileImgUrl, userItem)
        if(activity != Activities.PROFILE)
            userItem.setOnClickListener { navigateToUserProfile() }
    }

    private fun setMenuItem(item: View, badgeVisibility: Int, label: String) {
        item.label.text = label
        item.notifications_counter.visibility = badgeVisibility
    }

    private fun setSelectedItemStyle(textView: View) {
        textView.label.setTextColor(Color.parseColor("#00a8ff"))
    }

    private fun signOut(context: Context) {
        Server
                .auth
                .signOut(CurrentUser.user.uid).enqueue(object: Callback {
                    override fun onResponse(call: Call, response: Response) {
                        onSignOutResponse(response,context)
                    }

                    override fun onFailure(call: Call, e: IOException) {
                        Log.d(MessengerActivity.TAG, "$e.message")
                    }
                })
    }

    private fun onSignOutResponse(response: Response, context: Context) {
        if(response.isSuccessful) {
            val resp = response.body!!.string()
            val uid = GsonBuilder().create().fromJson(resp, Uid::class.java)
            Log.d(MessengerActivity.TAG, uid.uid)
            runOnUiThread {
                Toast.makeText(context, CurrentUser.user.username + " is disconnected", Toast.LENGTH_SHORT).show()
                SocketSingleton.disconnect()
                navigateToSignIn()
            }
        } else {
            runOnUiThread {
                Toast.makeText(context, " Something went wrong, could not  disconnect", Toast.LENGTH_SHORT).show()
            }
        }
    }

    protected fun notify(cid: String) {
        Sound.play(this, Sounds.NOTIFICATION, 0)
        if(!Notification.unreadConvosId.contains(cid))
        {
            Notification.unreadConvosId.add(cid)
        }
    }

    private val newMessageVM = MessagesViewModel()
    protected fun onNewMessages(navbar: View) {
        setNotificationCounter(navbar.msg_item.notifications_counter)
        newMessageVM.newMessage.observe(this, Observer {
            notify(it.cid)
            navbar.msg_item.notifications_counter.visibility = View.VISIBLE
        })
    }

    protected fun setNotificationCounter(counter: View){
        if(Notification.unreadConvosId.size > 0) { counter.visibility = View.VISIBLE }
        else { counter.visibility = View.GONE }
    }
}