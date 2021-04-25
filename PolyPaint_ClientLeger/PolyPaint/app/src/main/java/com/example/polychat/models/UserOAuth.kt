package com.example.polychat.models

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
data class UserOAuth(val user: User?, val isUsingOAuth: Boolean): Parcelable