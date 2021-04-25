package com.example.polychat.services.serverServices

import android.content.Context
import android.net.ConnectivityManager
import android.net.NetworkInfo

class ConnectivityManager {
    /**
     * todo: - Should be changed to BroadCast Receiver
     * */
    companion object {
        /**
         * @Important
         * This method checks only if the device is using a network.
         * It does not define the network state.
         * */
        fun isConnectToNetwork(context: Context): Boolean {
            val connMgr: ConnectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
            val networkInfo: NetworkInfo? = connMgr.activeNetworkInfo
            return networkInfo?.isConnected == true
        }
    }
}