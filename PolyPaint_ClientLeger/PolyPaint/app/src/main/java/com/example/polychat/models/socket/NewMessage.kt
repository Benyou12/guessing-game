package com.example.polychat.models.socket

import com.example.polychat.models.Message

data class NewMessage(val cid: String = "", val message: Message = Message())