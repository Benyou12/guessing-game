package com.example.polychat.services.serverServices

import android.icu.text.SimpleDateFormat
import java.util.*

class Time {
    companion object {

        fun getFormattedTime(timestamp: Long): String{
            val date = Date(timestamp)
            val timeFormatter = SimpleDateFormat("dd-MM-YYY HH:mm:ss")
            return timeFormatter.format(date)
        }

    }
}