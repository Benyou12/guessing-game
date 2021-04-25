package com.example.polychat.settings

import android.os.Bundle
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.models.Activities
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.UIMode
import com.example.polychat.viewModel.UserViewModel
import com.example.polychat.views.stats.BadgeItem
import com.example.polychat.views.stats.ConnexionItem
import com.example.polychat.views.stats.GamePlayedItem
import com.squareup.picasso.Picasso
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.layout_history.view.*
import kotlinx.android.synthetic.main.layout_input_with_label.view.*
import kotlinx.android.synthetic.main.layout_profile.*
import kotlinx.android.synthetic.main.layout_user_infos.*
import kotlinx.android.synthetic.main.layout_user_stats.*
import kotlinx.android.synthetic.main.layout_user_stats.view.*

class UserProfile: BaseActivity() {

    private val userVM = UserViewModel()
    private var badgeAdapter = GroupAdapter<ViewHolder>()
    private var gamePlayedAdapter = GroupAdapter<ViewHolder>()
    private var connectionHistoryAdapter = GroupAdapter<ViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.layout_profile)
        onUserStatsReceived()
        onLogOutReceived()
        requestUserStats()
        setTags()
        setLanguage(Localization.langCode)
        onUserStatsReceived()
        requestUserStats()
        setRecyclerViewAdapter()
        setNavMenu(menu_bar_profile, Activities.PROFILE)
        setUserView()
        onNewMessages(menu_bar_profile)
    }

    private fun requestUserStats() {
        MessengerService.getCurrentUserStats(CurrentUser.user.uid)
    }

    private fun onUserStatsReceived() {
        userVM.userStats.observe(this, Observer {userStats ->
            CurrentUser.userStat = userStats
            setStats()
        })
    }

    private fun setStats() {
        setStatsView()
        displayBadges()
        displayGamesHistory()
        displayConnectionHistory()
    }

    private fun displayConnectionHistory() {
        connectionHistoryAdapter.clear()
        CurrentUser.userStat.auth_history!!.forEach { userAuthStats ->
            connectionHistoryAdapter.add(
                ConnexionItem(userAuthStats)
            )
        }
    }

    private fun displayGamesHistory() {
        gamePlayedAdapter.clear()
        CurrentUser.userStat.game_history!!.forEach { gameHistory ->
            gamePlayedAdapter.add(
                GamePlayedItem(gameHistory)
            )
        }
    }

    private fun displayBadges() {
        badgeAdapter.clear()
        CurrentUser.userStat.gamification!!.badges.forEach { userBadge ->
            badgeAdapter.add(
                BadgeItem(userBadge)
            )
        }
    }

    private fun setTags() {
        firstName.user_info_label.tag = "user_profile_firstName"
        lastName.user_info_label.tag = "user_profile_lastName"
        userName.user_info_label.tag = "user_profile_pseudo"
        email.user_info_label.tag = "user_profile_email"
        score_stats.user_info_label.tag = "user_profile_score"
        number_badge_earned.user_info_label.tag = "user_profile_number_badges"
        number_of_victory.user_info_label.tag = "user_profile_victories"
        number_of_defeats.user_info_label.tag = "user_profile_defeat"
        connexion_history.history_label.tag = "user_profile_connexion_history"
        game_played_history.history_label.tag = "user_profile_game_history"
        badges.history_label.tag = "user_profile_badges"
    }

    private fun setLanguage(langCode: LangCode) {
        Localization.langCode = langCode
        Localization.setTextView(
            firstName.user_info_label, lastName.user_info_label,userName.user_info_label,email.user_info_label,
            score_stats.user_info_label,number_badge_earned.user_info_label,number_of_victory.user_info_label,
            number_of_defeats.user_info_label, connexion_history.history_label,
            game_played_history.history_label,badges.history_label)
    }

    private fun setStatsView() {
        score_stats.apply {
            text_view.text = CurrentUser.userStat.gamification!!.points.toString()
        }
        number_of_victory.apply {
            text_view.text = CurrentUser.userStat.game_stats!!.victories.toString()
        }
        number_of_defeats.apply {
            text_view.text = CurrentUser.userStat.game_stats!!.failures.toString()
        }
        number_badge_earned.apply {
            text_view.text = CurrentUser.userStat.gamification!!.badges.size.toString()
        }
    }

    private fun setUserView() {
        setProfileImage()
        userInfos.apply {
            userName.apply {
                text_view.text = CurrentUser.user.username
            }
            firstName.apply {
                text_view.text = CurrentUser.user.firstName
            }
            lastName.apply {
                text_view.text = CurrentUser.user.lastName
            }
            email.apply {
                text_view.text = CurrentUser.user.email
            }
        }
    }

    private fun setProfileImage() {
        Picasso.get().load(CurrentUser.user.profileImgUrl).into(profileImg)
    }

    private fun setRecyclerViewAdapter() {
        userStats.badges.history_recycler_view.adapter = badgeAdapter
        userStats.game_played_history.history_recycler_view.adapter = gamePlayedAdapter
        userStats.connexion_history.history_recycler_view.adapter = connectionHistoryAdapter
    }
}