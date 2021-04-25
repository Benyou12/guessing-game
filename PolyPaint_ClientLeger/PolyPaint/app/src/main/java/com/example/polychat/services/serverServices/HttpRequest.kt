package com.example.polychat.services.serverServices

import okhttp3.Call
import okhttp3.Callback
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody.Companion.toRequestBody


class HttpRequest(client: OkHttpClient) {
    private val mediaType = "application/json; charset=utf-8".toMediaType()
    var client = OkHttpClient()

    init {
        this.client = client
    }
    fun get(url: String):  Call
    {
        val request: Request = Request.Builder()
            .url(url)
            .build()

        return client.newCall(request)
    }

    fun post(url: String, json: String): Call
    {
        val requestBody = json.toRequestBody(mediaType)
        val request = Request.Builder()
            .url(url)
            .post(requestBody)
            .build()

        return client.newCall(request)
    }
}