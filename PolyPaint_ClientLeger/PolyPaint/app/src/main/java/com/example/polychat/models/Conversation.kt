package com.example.polychat.models

class Conversation(
    val cid: String,
    val convName: String?,
    val timestamp: Long,
    val updatedTimestamp: Long?,
    var uids: ArrayList<String>,
    val messages: ArrayList<Message>?,
    val users: ArrayList<User>?,
    val convImgUrl: String?,
    val updateAction: Int?)

enum class CONVERSATION_UPDATE(val value: Int){
    DEFAULT(0),
    CREATED(1),
    USER_ADDED(2),
    USER_REMOVED(3),
    UPDATED(4),
    DELETED(5)
}