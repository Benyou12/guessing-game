package com.example.polychat.services.util

import androidx.appcompat.app.AppCompatDelegate
import com.example.polychat.R

class UIMode {
    companion object {
        fun set(ctx: android.content.Context) {
            if (AppCompatDelegate.getDefaultNightMode() == AppCompatDelegate.MODE_NIGHT_YES) {
                ctx.setTheme(R.style.darktheme)
            } else {
                ctx.setTheme(R.style.AppTheme)
            }
        }
    }
}