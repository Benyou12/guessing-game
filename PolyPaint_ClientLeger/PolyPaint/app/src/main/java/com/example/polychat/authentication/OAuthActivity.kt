package com.example.polychat.authentication

import android.content.Intent
import android.os.Bundle
import android.view.View
import com.example.polychat.R
import java.util.*
import android.webkit.WebView
import androidx.lifecycle.Observer
import com.example.polychat.base.BaseActivity
import com.example.polychat.services.serverServices.SocketSingleton
import com.example.polychat.viewModel.UserViewModel
import android.webkit.WebViewClient
import com.example.polychat.models.SignInProvider
import com.example.polychat.models.User
import com.example.polychat.models.UserOAuth
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.util.UIMode
import kotlinx.android.synthetic.main.activity_oauth.*


class OAuthActivity : BaseActivity() {
    private lateinit var authenticationToken: String
    private lateinit var browser: WebView
    private val userVM = UserViewModel()
    private lateinit var  chosenProvider: SignInProvider

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_oauth)
        setProvider()
        onAuth()
        connectSocket()
        getWebView()
        loginWithOtherProvider()
        btn_back_sign_in.setOnClickListener {
            navigateToSignIn()
        }
    }

    private fun connectSocket() {
        SocketSingleton.connect()
    }

    private fun getWebView() {
        browser = findViewById(R.id.web_view_oauth)
    }

    private fun openBrowser(loginId: String, provider: String){
        val pageUrl = "https://log3900.lbacreations.com/auth/${provider}/?login_id=${loginId}"
        browser.settings.loadsImagesAutomatically = true
        browser.settings.javaScriptEnabled = true
        browser.webViewClient = WebViewClient()
        browser.loadUrl(pageUrl)
    }

    private fun loginWithOtherProvider(){
        val loginId = UUID.randomUUID().toString()
        authenticationToken = loginId
        openBrowser(loginId,chosenProvider.provider)

    }

    private fun onAuth() {
        userVM.userOAuth.observe(this, Observer{response ->
                if(response.isLogin){
                    if(response.login_id == authenticationToken){
                        SocketSingleton.joinRoom(response.user, this )
                        CurrentUser.user = response.user
                        navigateToHomeActivity()
                    }
                }
                else {
                    completeRegistration(response.user)
                }
        })
    }

    private fun completeRegistration(user: User) {
        val intent = Intent(this, SignUp::class.java)
        val userOAuth = UserOAuth(user,true)
        val userKey = "user"
        intent.putExtra(userKey, userOAuth)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
        startActivity(intent)
    }

    private fun setProvider() {
        val providerKey = "provider"
        chosenProvider = intent.getParcelableExtra(providerKey)?: return
    }


    enum class Provider(val value: String){
        Facebook("Facebook"),
        Google("Google"),
        Github("Github")
    }
}
