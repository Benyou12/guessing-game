package com.example.polychat.models

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
class User (
    val uid: String,
    val firstName: String,
    val lastName: String,
    var username: String,
    val email: String,
    val profileImgUrl: String
): Parcelable {
    constructor() : this( "", "","", "", "","")
}