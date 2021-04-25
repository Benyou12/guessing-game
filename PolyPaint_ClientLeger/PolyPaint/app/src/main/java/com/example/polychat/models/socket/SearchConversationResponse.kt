package com.example.polychat.models.socket

data class SearchConversationsResponse(
    val cid: String,
    val convName: String,
    val timestamp: Long,
    val uids: ArrayList<String>)