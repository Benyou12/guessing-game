package com.example.polychat.services

import com.example.polychat.services.serverServices.Authentification
import com.example.polychat.services.serverServices.ConnectivityManager
import com.example.polychat.services.serverServices.DataManager

class Server {

    companion object {
        /**
         * contains methods to signIn, signUp and SignOut
         * */
        val auth = Authentification
        /**
         * contains methods to manage CRUD(create, read, update and delete) available data on DataBase
         * */
        val dataManager = DataManager

        val connectivity = ConnectivityManager
    }
}

const val BASE_URL_SECOND = "https://log3900boubacar.herokuapp.com/"
const val BASE_URL: String =  "https://log3900.lbacreations.com/" //"https://donkeys4.herokuapp.com/" //"


enum class ServerUrlS(val url: String) {
    SingIn(BASE_URL + "auth/login" ),
    SingUP(BASE_URL + "user" ),
    SignOut(BASE_URL + "auth/logout"),
}
