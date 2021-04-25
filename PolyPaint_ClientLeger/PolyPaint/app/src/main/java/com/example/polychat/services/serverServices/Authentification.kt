package com.example.polychat.services.serverServices

import android.net.Uri
import com.example.polychat.models.LoginRequestModel
import com.example.polychat.models.SignUpRequestModel
import com.example.polychat.models.User
import com.example.polychat.models.socket.Uid
import com.example.polychat.services.ServerUrlS
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.firestore.FirebaseFirestore
import com.google.firebase.storage.FirebaseStorage
import com.google.gson.GsonBuilder
import nl.komponents.kovenant.Promise
import nl.komponents.kovenant.deferred
import nl.komponents.kovenant.reject
import nl.komponents.kovenant.resolve
import okhttp3.Call
import okhttp3.OkHttpClient
import java.util.*

class Authentification {
    companion object{
        fun signIn(email: String, password: String): Call
        {
            val url = ServerUrlS.SingIn.url
            val login = LoginRequestModel(email, password)
            val gson = GsonBuilder().create()
            val jsonLogin = gson.toJson(login).toString()
            val request = HttpRequest(client = OkHttpClient())

            return request.post(url,jsonLogin)
        }

        fun signUp(
            firstName: String,
            lastName: String,
            username: String,
            email: String,
            password: String,
            profileImageUri: String
        ): Call
        {
            val url = ServerUrlS.SingUP.url
            val signUp = SignUpRequestModel(firstName,lastName, username,email, password, profileImageUri)
            val gson = GsonBuilder().create()
            val jsonSignUP = gson.toJson(signUp).toString()
            val request = HttpRequest(client = OkHttpClient())

            return request.post(url,jsonSignUP)
        }

        fun signOut(uid: String): Call {
            val url = ServerUrlS.SignOut.url
            val uId = Uid(uid)
            val jsonUid = GsonBuilder().create().toJson(uId).toString()
            val request = HttpRequest(client = OkHttpClient())

            return request.post(url,jsonUid)
        }

        /**/
        private fun createUserWithEmailAndPassword(
            email: String,
            password: String
        ): Promise<Unit, Throwable> {
            val deferred = deferred<Unit, Throwable>()
            FirebaseAuth.getInstance().createUserWithEmailAndPassword(email, password)
                .addOnCompleteListener {
                    if (!it.isSuccessful) return@addOnCompleteListener
                    deferred.resolve()
                }
                .addOnFailureListener {
                    deferred.reject(it)
                }
            return deferred.promise
        }

        /**/
        fun uploadImage(imageUri: Uri): Promise<String, Throwable> {
            val deferred = deferred<String, Throwable>()
            val fileName = UUID.randomUUID().toString()
            val refPath = "/images/$fileName"
            val ref = FirebaseStorage.getInstance().getReference(refPath)
            ref.putFile(imageUri)
                .addOnSuccessListener {
                    ref.downloadUrl.addOnSuccessListener { imageUrl ->
                        deferred.resolve(imageUrl.toString())
                    }
                }
                .addOnFailureListener {
                    deferred.reject(it)
                }
            return deferred.promise
        }

        private fun addUserToDatabase(
            firstName: String,
            lastName: String,
            userName: String,
            email: String,
            profileImgUrl: String
        ): Promise<User, Unit> {
            val deferred = deferred<User, Unit>()
            // get the user's uid
            val uid = FirebaseAuth.getInstance().uid
            if (uid == null) {
                deferred.reject()
                return deferred.promise
            }
            val user = User(uid, firstName, lastName, userName, email, profileImgUrl)
            addUser(user)
                .success {
                    deferred.resolve(user)
                }
                .fail {
                    deferred.reject()
                }
            return deferred.promise
        }

        /**/
        private fun addUser(user: User): Promise<String, String> {
            val db = FirebaseFirestore.getInstance()
            val deferred = deferred<String, String>()
            db.collection("users").document(user.uid).set(user)
                .addOnSuccessListener {
                    // resolve promise
                    deferred.resolve("User Added")
                }
                .addOnCanceledListener {
                    deferred.reject("Something went wrong ! Please try again")
                }
                .addOnFailureListener { e ->
                    // reject promise
                    deferred.reject("Error adding document $e")
                }
            return deferred.promise
        }

    }
}