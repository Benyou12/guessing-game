package com.example.polychat.models

class Message(val mid: String, val user: User, var text: String, val timestamp: Long) {
    constructor() : this("", User(),"", 0)
}