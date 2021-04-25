package com.example.polychat.services.util

import android.content.Context
import android.media.AudioAttributes
import android.media.SoundPool
import com.example.polychat.models.Sounds

object Sound {

    private var loop = 0
    private val audioAttributes = AudioAttributes.Builder()
            .setUsage(AudioAttributes.USAGE_ASSISTANCE_SONIFICATION)
            .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
            .build()

    private val sp = SoundPool.Builder()
            .setMaxStreams(6)
            .setAudioAttributes(audioAttributes)
            .build()

    fun load() {
        sp.setOnLoadCompleteListener { _, _sampleId, _ ->
            sp.play(_sampleId, 1.0f, 1.0f, 1, loop, 1.0F)
            loop = 0
        }
    }

    fun play(ctx: Context, sound: Sounds, loop: Int) {
        sp.load(ctx, sound.id, 1)
        this.loop = loop
    }
}