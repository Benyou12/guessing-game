package com.example.polychat.models.socket

import com.example.polychat.models.User

data class IOAuthResponse(
    val isLogin: Boolean,
    val login_id: String,
    val user: User
)
