package com.example.polychat.services.util

/*
*  Class contains very useful String utility methods
* */
class StringUtility {

    companion object {
        /*
        *  Take a String and add a random number to it
        *  Example: Jack -> Jack_9808
        * */
        fun getRandomUserName(str: String): String {
            val magnitude = 1000
            val separator = "_"
            return str + separator + (Math.random()* magnitude).toInt()
        }
    }

}