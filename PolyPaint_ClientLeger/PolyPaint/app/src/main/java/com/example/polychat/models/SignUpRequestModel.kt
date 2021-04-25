package com.example.polychat.models

data class SignUpRequestModel(
    val firstName: String = "",
    val lastName: String = "",
    val username: String = "",
    val email: String = "",
    val password: String = "",
    val profileImgUrl: String = ""
)