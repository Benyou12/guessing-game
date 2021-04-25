package com.example.polychat.services.util
import com.example.polychat.models.Drawing.Stroke

class StrokeQueue (list:MutableList<Stroke>){

    var strokes:MutableList<Stroke> = list

    fun isEmpty():Boolean = strokes.isEmpty()

    fun size():Int = strokes.count()

    override  fun toString() = strokes.toString()

    fun enqueue(element:Stroke){
        strokes.add(element)
    }

    fun dequeue():Stroke?{
        return if (this.isEmpty()){
            null
        } else {
            strokes.removeAt(0)
        }
    }

    fun peek():Stroke?{
        return strokes[0]
    }
}