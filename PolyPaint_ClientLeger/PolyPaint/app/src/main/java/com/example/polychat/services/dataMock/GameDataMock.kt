package com.example.polychat.services.dataMock

import com.example.polychat.framgmentsViews.*
import com.example.polychat.models.User
import com.example.polychat.models.game.*

class GameDataMock {
    companion object {
        fun scoreboard(): ScoreboardData {
            val playerOneData = PlayerData(
                    "John12",
                    "Drawer",
                    "https://miro.medium.com/fit/c/256/256/2*egHHjM2mAMcdrgMoHsDfIg.jpeg"
            )
            val userTeam = TeamData(
                    "Your team",
                    playerOneData,
                    playerOneData,
                    "Points",
                    "0"
            )

            val opponent = TeamData(
                    "Opponents",
                    playerOneData,
                    playerOneData,
                    "Points",
                    "0"
            )

            return ScoreboardData("ScoreboardFragment", opponent, userTeam, arrayListOf(), false)
        }
    }
}