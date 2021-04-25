package com.example.polychat.models.Drawing

import android.graphics.Paint
import java.util.*
import kotlin.collections.ArrayList

data class Stroke(
        val _id: String,
        var coordinates: ArrayList<Pixel>,
        val color: String,
        val canvas_id: String,
        val cap: Cap,
        val size: Int,
        var toDelete: Boolean = false
) {

    companion object {
        fun newInstance(_id: String?, canvas_id: String, color: String, cap: Cap, size: Int, toDelete: Boolean): Stroke {
            val id = _id ?: UUID.randomUUID().toString()
            val coordinates = ArrayList<Pixel>()
            return Stroke(id, coordinates, color, canvas_id, cap, size, toDelete)
        }

        fun toServerCapType(androidCap: Paint.Cap): Cap {
            return if(androidCap == Paint.Cap.ROUND) Cap.ROUND else Cap.SQUARE
        }
    }

    fun toAndroidCapType(): Paint.Cap {
        return if(cap == Cap.ROUND) Paint.Cap.ROUND else Paint.Cap.SQUARE
    }

    fun addPixel(x: Float, y:Float) {
        coordinates.add(Pixel(x,y))
    }
}

enum class Cap(val value: Int) {
    SQUARE(0),
    ROUND(1),
}
