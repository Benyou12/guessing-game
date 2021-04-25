package com.example.polychat.authentication

import android.app.Activity
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import okhttp3.Callback
import android.util.Log
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.models.User
import com.example.polychat.services.Server
import com.example.polychat.services.serverServices.SocketSingleton
import com.example.polychat.views.DialogBox
import com.example.polychat.models.UserOAuth
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.util.UIMode
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.google.gson.GsonBuilder
import com.squareup.picasso.Picasso
import kotlinx.android.synthetic.main.activity_sign_up.*
import okhttp3.Call
import okhttp3.Response
import java.io.IOException

class SignUp : BaseActivity() {

    companion object {
        private const val TAG: String = "#SignUp"
    }

    private var profileImageUri: Uri? = null
    private var userOAuth: UserOAuth = UserOAuth(null,false)
    private var profileImageURL = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setUser()
        setContentView(R.layout.activity_sign_up)
        setUserOAuthInfos()
        setLanguage(Localization.langCode)
        selectProfileImage()
        button_already_have_an_account.setOnClickListener {
            navigateToSignInActivity()
        }
        // sign up
        button_sign_up.setOnClickListener {
           if(isConnected()) signUp()
           else  DialogBox.display(
               context = this,
               title = resources.getString(R.string.message_box_title),
               message = resources.getString(R.string.connection_error_message))
        }
    }

    private fun selectProfileImage() {
        if(!userOAuth.isUsingOAuth)
            button_profile_img.setOnClickListener { openImageFolder() }
    }

    private fun setUserOAuthInfos() {
        if(userOAuth.isUsingOAuth) {
            button_profile_img.alpha = 0f
            profileImageURL = userOAuth.user!!.profileImgUrl
            Picasso.get().load(profileImageURL).into(view_profile_image)
            input_firstName.setText(userOAuth.user!!.firstName)
            input_lastName.setText(userOAuth.user!!.lastName)
            input_email.setText(userOAuth.user!!.email)
        }
    }

    private fun setUser() {
        val user = "user"
        userOAuth = intent.getParcelableExtra(user) ?: return
    }

    fun setLanguage(langCode: LangCode) {
        Localization.langCode = langCode
        Localization.setButton(button_sign_up, button_already_have_an_account, button_profile_img)
        Localization.setInput(input_username, input_email, input_firstName, input_lastName, input_password)
    }

    private fun signUp() {
        val firstName = input_firstName.text.toString()
        val lastName = input_lastName.text.toString()
        val userName = input_username.text.toString()
        val email    = input_email.text.toString()
        val password = input_password.text.toString()
        if(isEmpty(firstName, lastName, userName, email, password)) {
            DialogBox.display(
                context = this,
                title = resources.getString(R.string.message_box_title),
                message = resources.getString(R.string.error_message_fill_in_fields))

            return
        }
        if(profileImageUri == null && !userOAuth.isUsingOAuth)
        {
            DialogBox.display(
                context = this,
                title = resources.getString(R.string.message_box_title),
                message = resources.getString(R.string.profile_image_error))

            return
        }

        if(!userOAuth.isUsingOAuth)
        {
            Server
                .auth
                .uploadImage(profileImageUri!!)
                .success {url ->
                    signUp(firstName, lastName, userName, email, password, url)
                }
                .fail {
                    Log.d(TAG, "Photo not uploaded")
                }
        }
        else {
            signUp(firstName, lastName, userName, email, password, profileImageURL)
        }
    }

    val thisActivity = this
    private fun signUp(firstName: String, lastName: String, userName: String, email: String, password: String, profileImageUrl: String) {
        Server
            .auth
            .signUp(firstName, lastName, userName, email, password, profileImageUrl)
            .enqueue(object: Callback {
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
                            SocketSingleton.joinRoom(CurrentUser.user, thisActivity)
                            Log.d(TAG, CurrentUser.user.profileImgUrl)
                            navigateToTutorial()
                        }
                        else
                        {
                            DialogBox.display(
                                context = thisActivity,
                                title = resources.getString(R.string.message_box_title),
                                message = resp)
                        }
                    }
                }

                override fun onFailure(call: Call, e: IOException)
                {
                    Log.d(TAG, "On failure - error : ${e.message}")
                }
            })
    }

    private fun isEmpty(vararg str: String): Boolean {
        for (s in str)
        {
            if (s.isEmpty()) return true
        }
        return false
    }

    /** Image Selector manager */
    private fun openImageFolder() {
        val intent = Intent(Intent.ACTION_PICK)
        intent.type = resources.getString(R.string.intent_type)
        startActivityForResult(intent,0)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(requestCode == 0 && resultCode == Activity.RESULT_OK && data != null) {
            val imgUri = data.data ?: return
            profileImageUri = imgUri
            setProfileImage(imgUri)
        }
    }

    private fun setProfileImage(imgUri: Uri) {
        val height = view_profile_image.height
        val width = view_profile_image.width
        Picasso
            .get()
            .load(imgUri)
            .resize(width, height)
            .centerCrop()
            .into(view_profile_image)
        button_profile_img.alpha = 0f
    }
    /** Connectivity Manager*/
    private fun isConnected(): Boolean {
        return Server.connectivity.isConnectToNetwork(this)
    }

}

