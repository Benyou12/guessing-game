package com.example.polychat.base

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.ImageView
import androidx.appcompat.app.AppCompatDelegate
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.models.Activities
import com.example.polychat.models.userModels.UserBadge
import com.example.polychat.models.userModels.UserGameStats
import com.example.polychat.services.CurrentUser
import com.example.polychat.services.Labels
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.UIMode
import com.example.polychat.viewModel.UserViewModel
import kotlinx.android.synthetic.main.acitivity_home.*
import kotlinx.android.synthetic.main.layout_3_badges.view.*
import kotlinx.android.synthetic.main.layout_options.*
import kotlinx.android.synthetic.main.layout_stats.view.*
import kotlinx.android.synthetic.main.layout_team_card.view.text_view_label
import kotlinx.android.synthetic.main.layout_user_stats_badges.view.*
import kotlinx.android.synthetic.main.user_stats.view.*

class HomeActivity : BaseActivity() {

    private val userVM = UserViewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.acitivity_home)
        setNavMenu(home_navBar, Activities.HOME)
        onUserStatResponse()
        requestCurrentUserStats()
        onLogOutReceived()
        setUIModeButtons()
        setLanguageButtons()
        setGameButton()
        onNewMessages(home_navBar)
        home_stats.alpha = 0f
    }

    private fun setGameButton() {
        button_classic_mode.setOnClickListener {
            navigateToGame()
        }
        btn_tuto.text = Labels.TUTORIAL_BTN.getValue()
        btn_tuto.setOnClickListener {
            navigateToTutorial()
        }
    }

    private fun setLanguageButtons() {
        lang_en.setOnClickListener {
            if(Localization.langCode == LangCode.EN) return@setOnClickListener
            updateLanguage(LangCode.EN)
            btn_tuto.text = Labels.TUTORIAL_BTN.getValue()
        }
        lang_fr.setOnClickListener {
            if(Localization.langCode == LangCode.FR) return@setOnClickListener
            updateLanguage(LangCode.FR)
            btn_tuto.text = Labels.TUTORIAL_BTN.getValue()
        }
    }

    private fun updateLanguage(langCode: LangCode) {
        Localization.langCode = langCode
        setNavMenu(home_navBar, Activities.HOME)
        showSelectedUserInfo()
    }

    private fun setUIModeButtons() {
        darkMode.setOnClickListener {
            val isDarkMode = AppCompatDelegate.getDefaultNightMode()== AppCompatDelegate.MODE_NIGHT_YES
            if(isDarkMode) return@setOnClickListener
            AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES)
            restartApp()
        }
        lightMode.setOnClickListener {
            val isLightMode = AppCompatDelegate.getDefaultNightMode()== AppCompatDelegate.MODE_NIGHT_NO
            if(isLightMode) return@setOnClickListener
            AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO)
            restartApp()
        }
    }

    private fun restartApp() {
        val intent = Intent(this, HomeActivity::class.java)
        startActivity(intent)
    }

    private fun requestCurrentUserStats() {
        MessengerService.getCurrentUserStats(CurrentUser.user.uid)
    }

    private fun onUserStatResponse() {
        userVM.userStats.observe(this, Observer {
            it ?: return@Observer
            CurrentUser.userStat = it
            showSelectedUserInfo()
        })
    }

    private fun showSelectedUserInfo() {
        home_spinner.visibility = View.GONE
        home_stats.alpha = 1f
        button_classic_mode.text = Labels.BUTTON_GAME_CLASSIC.getValue()
        home_stats.p_user_name.text = CurrentUser.userStat.username
        loadUrlIntoView(CurrentUser.userStat.profileImgUrl, home_stats.p_profileImg)
        CurrentUser.userStat.gamification ?: return
        val badges = CurrentUser.userStat.gamification!!.badges
        displayBadges(badges, home_stats.badges.p_badge, home_stats.badges.p_badge2, home_stats.badges.p_badge3)
        val userStats = CurrentUser.userStat.game_stats
        displayUserStat(userStats!!, badges.size)
    }

    private fun displayBadges(badges: ArrayList<UserBadge>,
                              pBadge: ImageView,
                              pBadge2: ImageView,
                              pBadge3: ImageView) {
        val size = badges.size
        when
        {
            size > 2  ->
            {
                displayBadge(badges[0], pBadge)
                displayBadge(badges[1], pBadge2)
                displayBadge(badges[2], pBadge3)
            }
            size == 2 ->
            {
                displayBadge(badges[0], pBadge)
                displayBadge(badges[1], pBadge2)
            }
            size == 1 ->
            {
                displayBadge(badges[0], pBadge)
            }
        }
    }

    private fun displayBadge(userBadge: UserBadge, pBadge: ImageView) {
        val bUrl = if (Localization.langCode == LangCode.EN) userBadge.badge.img_en
        else userBadge.badge.img_fr
        loadUrlIntoView(bUrl, pBadge)
    }

    private fun displayUserStat(userStats: UserGameStats, nbOfBadges: Int) {
        home_stats.stat1.text_view_label.text = Labels.PLAYED_LABEL.getValue()
        home_stats.stat1.text_view_value.text = userStats.total_games_played.toString()
        home_stats.stat2.text_view_label.text = Labels.WIN_LABEL.getValue()
        home_stats.stat2.text_view_value.text = userStats.victories.toString()
        home_stats.stat3.text_view_label.text = Labels.BADGES_LABEL.getValue()
        home_stats.stat3.text_view_value.text = nbOfBadges.toString()
    }
}