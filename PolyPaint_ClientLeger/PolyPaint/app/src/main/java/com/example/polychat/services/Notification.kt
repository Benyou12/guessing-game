package com.example.polychat.services

class Notification {
    companion object{
        var unreadConvosId: MutableSet<String> = mutableSetOf()
    }
}