package com.example.polychat.services.util

import android.widget.AutoCompleteTextView
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import org.json.JSONObject

enum class LangCode(val code: String){
    EN("en"),
    FR("fr")
}

object Localization {
    private lateinit var mValues: String
    var langCode: LangCode = LangCode.EN

    fun init(string: String) {
        mValues = string
    }

    fun setTextView(vararg textViews: TextView) {
        textViews.forEach {
            val id = it.tag as String
            val fieldValues = JSONObject(mValues)[id]
            it.text = JSONObject(fieldValues.toString())[langCode.code].toString()
        }
    }

    fun setButton(vararg buttons: Button) {
        buttons.forEach {
            val id = it.tag as String
            val fieldValues = JSONObject(mValues)[id]
            it.text = JSONObject(fieldValues.toString())[langCode.code].toString()
        }
    }

    fun setInput(vararg editTexts: EditText) {
        editTexts.forEach{
            val id = it.tag as String
            val fieldValues = JSONObject(mValues)[id]
            it.hint = JSONObject(fieldValues.toString())[langCode.code].toString()
        }
    }

    fun setAutoCompleteTextView(vararg autoCompleteTextViews: AutoCompleteTextView) {
        autoCompleteTextViews.forEach {
            val id = it.tag as String
            val fieldValues = JSONObject(mValues)[id]
            it.hint = JSONObject(fieldValues.toString())[langCode.code].toString()
        }
    }

    fun locaLizedAlertDialog(vararg elements: String): MutableMap<String, String> {
        val  localiZedElements: MutableMap<String,String> = mutableMapOf()
        elements.forEach {
            val filedValue = JSONObject(mValues)[it]
            localiZedElements[it] = JSONObject(filedValue.toString())[langCode.code].toString()
        }

        return localiZedElements
    }
}