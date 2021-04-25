package com.example.polychat.services

import com.example.polychat.models.User
import com.example.polychat.models.userModels.UserStats

class CurrentUser {
    companion object {
        lateinit var userStat: UserStats
        lateinit var user: User
    }
}