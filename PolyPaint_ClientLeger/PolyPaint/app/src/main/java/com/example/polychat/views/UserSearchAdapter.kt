package com.example.polychat.views

import android.annotation.SuppressLint
import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.Filter
import android.widget.Filterable
import android.widget.TextView
import androidx.annotation.LayoutRes
import com.example.polychat.models.User
import java.util.*

class UserSearchAdapter(
    context: Context,
    @LayoutRes private val layoutResource: Int,
    private val users: List<User>):
    ArrayAdapter<User>(context, layoutResource, users),
    Filterable {
    private var searchUsers: List<User> = users

    override fun getCount(): Int {
        return searchUsers.size
    }

    override fun getItem(p0: Int): User? {
        return searchUsers[p0]
    }

    @SuppressLint("ViewHolder")
    override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
        val view: TextView = convertView as TextView? ?: LayoutInflater.from(context).inflate(layoutResource, parent, false) as TextView
        view.text = searchUsers[position].username
        return view
    }
    override fun getFilter(): Filter {
        return object : Filter() {
            override fun publishResults(charSequence: CharSequence?, filterResults: FilterResults) {
                searchUsers = filterResults.values as List<User>
                notifyDataSetChanged()
            }

            override fun performFiltering(charSequence: CharSequence?): FilterResults {
                val queryString = charSequence?.toString()?.toLowerCase(Locale.ROOT)

                val filterResults = FilterResults()
                filterResults.values = if (queryString==null || queryString.isEmpty())
                    users
                else
                    users.filter {
                        it.username.toLowerCase(Locale.ROOT).contains(queryString)
                    }
                return filterResults
            }
        }
    }
}