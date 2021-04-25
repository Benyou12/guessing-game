package com.example.polychat.settings

import android.graphics.Color
import android.os.Bundle
import android.view.View
import android.widget.ImageView
import androidx.lifecycle.Observer
import com.example.polychat.R
import com.example.polychat.base.BaseActivity
import com.example.polychat.models.Activities
import com.example.polychat.models.userModels.UserBadge
import com.example.polychat.models.userModels.UserGameStats
import com.example.polychat.models.userModels.UserStats
import com.example.polychat.services.Labels
import com.example.polychat.services.messenger.MessengerService
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization
import com.example.polychat.services.util.UIMode
import com.example.polychat.viewModel.UserViewModel
import com.example.polychat.views.UserItem
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.activity_public_profile.*
import kotlinx.android.synthetic.main.layout_3_badges.view.*
import kotlinx.android.synthetic.main.layout_history.view.*
import kotlinx.android.synthetic.main.layout_stats.view.*
import kotlinx.android.synthetic.main.layout_team_card.view.text_view_label
import kotlinx.android.synthetic.main.layout_user_stats_badges.view.*
import kotlinx.android.synthetic.main.menu_item_messenger.view.*
import kotlinx.android.synthetic.main.navbar_game.view.*
import kotlinx.android.synthetic.main.user_stats.view.*

class PublicProfile : BaseActivity() {

    private var selectedUser: UserStats? = null
    private var allUsers: Array<UserStats>? = null
    private var adapter = GroupAdapter<ViewHolder>()
    private var userViewModel = UserViewModel()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        UIMode.set(this)
        setContentView(R.layout.activity_public_profile)
        setUsersRecyclerViewAdapter()
        requestAllUsers()
        onLogOutReceived()
        onUsersResponse()
        setView()
        setNavMenu(p_menu_bar_profile, Activities.USERS)
        onNewMessages(p_menu_bar_profile)
        p_users.alpha = 0f
    }

    private fun setView() {
        setCurtain(show = true)
        p_users.history_label.text = Labels.USER_LABEL.getValue()
    }

    private fun setCurtain(show: Boolean) {
        p_curtain.visibility = if(show) View.VISIBLE else View.GONE
    }

    private fun requestAllUsers() {
        MessengerService.getAllUsers()
    }

    private fun onUsersResponse() {
        userViewModel.publicUsers.observe(this, Observer {
            allUsers = it
            addUsersToAdapter(it)
        })
    }

    private fun showSelectedUserInfo() {
        selectedUser ?: return
        stats.p_user_name.text = selectedUser!!.username
        loadUrlIntoView(selectedUser!!.profileImgUrl, stats.p_profileImg)
        selectedUser!!.gamification ?: return
        val badges = selectedUser!!.gamification!!.badges
        displayBadges(badges, stats.badges.p_badge, stats.badges.p_badge2, stats.badges.p_badge3)
        val userStats = selectedUser!!.game_stats
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
                pBadge3.visibility = View.GONE
            }
            size == 1 ->
            {
                displayBadge(badges[0], pBadge)
                pBadge2.visibility = View.GONE
                pBadge3.visibility = View.GONE
            }
            size == 0 ->
            {
                pBadge.visibility = View.GONE
                pBadge2.visibility = View.GONE
                pBadge3.visibility = View.GONE
            }
        }
    }

    private fun displayBadge(userBadge: UserBadge, pBadge: ImageView) {
        pBadge.visibility = View.VISIBLE
        val bUrl = if (Localization.langCode == LangCode.EN) userBadge.badge.img_en
        else userBadge.badge.img_fr
        loadUrlIntoView(bUrl, pBadge)
    }

    private fun displayUserStat(userStats: UserGameStats, nbOfBadges: Int) {
        stats.stat1.text_view_label.text = Labels.PLAYED_LABEL.getValue()
        stats.stat1.text_view_value.text = userStats.total_games_played.toString()
        stats.stat2.text_view_label.text = Labels.WIN_LABEL.getValue()
        stats.stat2.text_view_value.text = userStats.victories.toString()
        stats.stat3.text_view_label.text = Labels.BADGES_LABEL.getValue()
        stats.stat3.text_view_value.text = nbOfBadges.toString()
    }

    private fun addUsersToAdapter(allUsers: Array<UserStats>) {
        allUsers.forEach { user ->
            adapter.add(
                    UserItem(user)
            )
        }
        p_users.alpha = 1f
        stats_spinner.visibility = View.GONE
        onItemClicked()
    }

    private fun onItemClicked() {
        adapter.setOnItemClickListener { item, _ ->
            val userItem = item as UserItem
            selectedUser = userItem.user
            setCurtain(show = false)
            showSelectedUserInfo()
        }
    }

    private fun setUsersRecyclerViewAdapter() {
        p_users.setBackgroundColor(Color.TRANSPARENT)
        p_users.history_recycler_view.adapter = adapter
    }
}
