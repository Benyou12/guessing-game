package com.example.polychat.models

import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization

data class Label(val fr: String, val en: String) {
    fun getValue(): String {
        return if(Localization.langCode.code == LangCode.FR.code) fr else en
    }
}
