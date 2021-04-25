package com.example.polychat.viewModel

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.polychat.models.Drawing.Stroke
import com.example.polychat.models.socket.ServerCanvas
import com.example.polychat.models.socket.SocketEvents
import com.example.polychat.services.serverServices.SocketSingleton
import com.google.gson.GsonBuilder

class CanvasViewModel : ViewModel(){
    private val _strokeUpdated = MutableLiveData<Stroke>()
    private val _canvas = MutableLiveData<ServerCanvas>()

    val canvas: LiveData<ServerCanvas>
        get() = _canvas

    val strokeUpdated: LiveData<Stroke>
        get() = _strokeUpdated

    init {
        onCanvasCreated()
        onStrokeReceived()
    }

    private fun onCanvasCreated() {
        SocketSingleton.socket.on(SocketEvents.CANVAS_CREATED.event) {
            val canvas: ServerCanvas = GsonBuilder().create().fromJson(it[0].toString(), ServerCanvas::class.java)
            _canvas.postValue(canvas)
        }
    }

    private fun onStrokeReceived() {
        SocketSingleton.socket.on(SocketEvents.STROKE_UPDATED.event) {
            val newStroke: Stroke = GsonBuilder().create().fromJson(it[0].toString(), Stroke::class.java)
            _strokeUpdated.postValue(newStroke)
        }
    }
}