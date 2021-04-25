package com.example.polychat.authentication

import android.content.Intent
import android.os.Bundle
import android.util.Log
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.models.SignInProvider
import com.example.polychat.models.User
import com.example.polychat.services.Server
import com.example.polychat.services.serverServices.SocketSingleton
import com.example.polychat.views.DialogBox
import com.example.polychat.services.util.UIMode
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.google.gson.GsonBuilder
import kotlinx.android.synthetic.main.activity_sign_in.*
import okhttp3.Call
import okhttp3.Callback
import okhttp3.Response
import java.io.IOException
import com.example.polychat.services.util.Sound
import com.example.polychat.services.CurrentUser

class SignIn : BaseActivity() {

    companion object{
        private const val TAG = "#SignIn"
    }

    private lateinit var provider: SignInProvider

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_sign_in)
        setLanguage(Localization.langCode)
        oAuthSignIn()
        Sound.load()
        button_sign_in.setOnClickListener {
            if(isConnected()) signIn()
            else {
                DialogBox.display(
                    context = this,
                    title = resources.getString(R.string.message_box_title),
                    message = resources.getString(R.string.connection_error_message))
            }
        }
        button_signUp.setOnClickListener {
            navigateToSignUpActivity()
        }
    }

    private fun oAuthSignIn() {
        img_view_fb.setOnClickListener {
            provider = SignInProvider(OAuthActivity.Provider.Facebook.value)
            navigateToWebView()
        }

        img_view_github.setOnClickListener {
            provider = SignInProvider(OAuthActivity.Provider.Github.value)
            navigateToWebView()
        }
    }

    private fun navigateToWebView() {
        val intent = Intent(this, OAuthActivity::class.java)
        val provideKey = "provider"
        intent.putExtra(provideKey, provider)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    private fun setLanguage(langCode: LangCode) {
        init()
        Localization.langCode = langCode
        Localization.setButton(button_signUp, button_sign_in)
        Localization.setTextView(welcome_Back)
        Localization.setInput(input_password_sign_in, input_email_sign_in)
    }

    fun init() {
        val inputStream = assets.open("strings.json")
        val langStrings = inputStream.bufferedReader().use { it.readText() }
        Localization.init(langStrings)
    }

    val thisActivity = this
    private fun signIn() {
        val self = this
        val email = input_email_sign_in.text.toString()
        val password = input_password_sign_in.text.toString()
        if (email.isEmpty() || password.isEmpty()) {
            DialogBox.display(
                context = this,
                title = resources.getString(R.string.message_box_title),
                message = resources.getString(R.string.error_message_fill_in_fields))
            return
        }

        Server
            .auth
            .signIn(email,password).enqueue(object: Callback{
                override fun onResponse(call: Call, response: Response)
                {
                  val resp = response.body!!.string()
                    runOnUiThread {
                        if (response.isSuccessful)
                        {
                            val gson = GsonBuilder().create()
                            CurrentUser.user = gson.fromJson(resp, User::class.java)
                            Log.d(TAG, CurrentUser.user.uid)
                            SocketSingleton.connect()
                            SocketSingleton.joinRoom(CurrentUser.user, thisActivity )
//                            navigateToUsersPage()
                            navigateToHomeActivity()
                        }
                        else
                        {
                            Log.d(TAG, "On response : $resp")
                            DialogBox.display(
                                context = self,
                                title = resources.getString(R.string.message_box_title),
                                message = resp
                            )
                        }
                    }
                }

                override fun onFailure(call: Call, e: IOException)
                {
                    Log.d(TAG, "On failure - echec : ${e.message}")
                }
            })
    }

    /** Connectivity Manager*/
    private fun isConnected(): Boolean {
        return Server.connectivity.isConnectToNetwork(this)
    }
}
