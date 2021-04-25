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
import com.example.polychat.models.Conversation
import java.util.*

@Suppress("UNCHECKED_CAST")
class ConversationSearchAdapter(
    context: Context,
    @LayoutRes private val layoutResource: Int,
    private val conversations: List<Conversation>):
    ArrayAdapter<Conversation>(context, layoutResource, conversations),
    Filterable {
    private var searchConversations: List<Conversation> = conversations

    override fun getCount(): Int {
        return searchConversations.size
    }

    override fun getItem(p0: Int): Conversation? {
        return searchConversations[p0]
    }

    @SuppressLint("ViewHolder")
    override fun getView(position: Int, convertView: View?, parent: ViewGroup): View {
        val view: TextView = convertView as TextView? ?: LayoutInflater.from(context).inflate(layoutResource, parent, false) as TextView
        view.text = searchConversations[position].convName
        return view
    }
    override fun getFilter(): Filter {
        return object : Filter() {
            override fun publishResults(charSequence: CharSequence?, filterResults: FilterResults) {
                searchConversations = filterResults.values as List<Conversation>
                notifyDataSetChanged()
            }

            override fun performFiltering(charSequence: CharSequence?): FilterResults {
                val queryString = charSequence?.toString()?.toLowerCase(Locale.ROOT)

                val filterResults = FilterResults()
                filterResults.values = if (queryString==null || queryString.isEmpty())
                    conversations
                else
                    conversations.filter {
                        it.convName!!.toLowerCase(Locale.ROOT).contains(queryString)
                    }
                return filterResults
            }
        }
    }
}