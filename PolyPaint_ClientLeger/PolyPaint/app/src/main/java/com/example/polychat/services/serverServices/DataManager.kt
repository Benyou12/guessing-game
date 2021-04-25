package com.example.polychat.services.serverServices

import com.example.polychat.models.Conversation
import com.example.polychat.models.Message
import com.example.polychat.models.User
import com.google.android.gms.tasks.Task
import com.google.firebase.firestore.*
import com.google.gson.GsonBuilder
import nl.komponents.kovenant.Promise
import nl.komponents.kovenant.deferred
import nl.komponents.kovenant.resolve
import nl.komponents.kovenant.task
import okhttp3.*
import java.io.IOException
import java.util.*
import kotlin.collections.ArrayList

class DataManager {
    companion object{
        val conversations = Conversations
        val users = Users
        val dataUtilies = DataUtilies
        const val GENERAL_CID = "general"
    }

}

class Conversations {
    companion object {

//        fun getConversationMessagesQuery(cid: String, orderBy: String): Query {
//            return FirebaseFirestore
//                .getInstance()
//                .collection("conversations")
//                .document(cid)
//                .collection("messages")
//                .orderBy(orderBy, Query.Direction.ASCENDING)
//        }

//        fun createConversation(users: ArrayList<User>, name: String): Promise<Unit, Throwable> {
//            val deferred = deferred<Unit, Throwable>()
//            if (users.isEmpty()) {
//                deferred.reject(Exception("Empty User's Array"))
//                return deferred.promise
//            }
//            val conversationId = UUID.randomUUID().toString()
//            val usersIds: ArrayList<String> = Users.getUsersIdsArray(users)
//            val timestamp = System.currentTimeMillis()
//            val conversation = Conversation(conversationId, usersIds, timestamp,null, name,null)
//            addConversation(conversation)
//                .addOnSuccessListener {
//                    task{
//                        users.forEach {
//                            addUserToConversation(it, conversationId)
//                        }
//                    }
//                    if ( deferred.promise.isFailure()) return@addOnSuccessListener
//                    deferred.resolve()
//                }
//                .addOnFailureListener {
//                    deferred.reject(it)
//                }
//            return deferred.promise
//        }

        fun addConversation(conversation: Conversation): Task<Void> {
            val collectionPath = "conversations"
            return FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(conversation.cid)
                .set(conversation)
        }

        fun getInitMessages(users: ArrayList<User>): ArrayList<Message> {
            val messages = ArrayList<Message>()
            users.forEach { user ->
                val msgText = user.firstName + " " + user.lastName + " " + "Joined the conversation"
                val message = createMessage(user, msgText)
                messages.add(message)
            }
            return messages
        }

        fun createMessage(user: User, text: String): Message {
            val timestamp = System.currentTimeMillis()
            val mid = UUID.randomUUID().toString()
            return Message( mid, user, text, timestamp)
        }

        fun addUserToConversation(user: User, cid: String): Task<Void> {
            val userCollectionPath = "conversations/$cid/users"
            return DataUtilies.addDocument(userCollectionPath, user.uid, user)
        }

        fun addMessageToConversation(message: Message, cid: String): Task<Void> {
            val messageCollectionPath = "conversations/$cid/messages"
            return DataUtilies.addDocument(messageCollectionPath, message.mid, message)
        }

//        fun getMessages(cid: String): Task<QuerySnapshot> {
//            val conversationsCollectionPath = "conversations"
//            val messagesCollectionPath = "messages"
//            return FirebaseFirestore
//                .getInstance()
//                .collection(conversationsCollectionPath)
//                .document(cid)
//                .collection(messagesCollectionPath)
//                .get()
//        }
//        fun getConversation(cid: String): Task<DocumentSnapshot> {
//            val collectionPath = "conversations"
//            return FirebaseFirestore
//                .getInstance()
//                .collection(collectionPath)
//                .document(cid)
//                .get()
//        }
//        fun getUserConversations(uid: String): Task<QuerySnapshot> {
//            val collectionPath = "conversations"
//            val fieldTarget = "uids"
//            return FirebaseFirestore
//                .getInstance()
//                .collection(collectionPath)
//                .whereArrayContains(fieldTarget, uid)
//                .get()
//        }
    }
}

class Users {
    companion object {
        fun getUser(uid: String): Task<DocumentSnapshot> {
            val collectionPath = "users"
            return  FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(uid)
                .get()
        }

        fun getUsersIdsArray(users: ArrayList<User>): ArrayList<String> {
            return ArrayList(users.map { it.uid })
        }
    }
}

class DataUtilies {
    companion object {

        fun addUidToGeneralConversation(uid: String): Task<Void> {
            val generalCid = "general"
            val collectionPath = "conversations"
            val fieldName = "uids"
            return FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(generalCid).update(fieldName, FieldValue.arrayUnion(uid))
        }

        fun addValuetoArray(collectionPath: String, documentPath: String, fieldName: String, value: String): Task<Void> {
            return FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(documentPath).update(fieldName, FieldValue.arrayUnion(value))
        }

        fun fetchServerTimestamp(): Promise<Long, Unit> {
            val deferred = deferred<Long, Unit>()
            val timestampUrl = "https://log3900-polychat.herokuapp.com/time/now"
            val request = Request.Builder().url(timestampUrl).build()
            val client = OkHttpClient()
            client.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                }
                override fun onResponse(call: Call, response: Response) {
                    val resp = response.body!!.string()
                    val gson = GsonBuilder().create()
                    val serverTimestamp = gson.fromJson(resp, ServerTimestamp::class.java)
                    deferred.resolve(serverTimestamp.timestamp)
                }
            })
            return deferred.promise
        }

        fun setField(collectionPath: String, documentPath: String ,fieldName: String, value: Any): Task<Void> {
            return FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(documentPath)
                .update(fieldName, value)
        }

        fun addDocument(collectionPath: String, documentPath: String, value: Any): Task<Void> {
            return FirebaseFirestore
                .getInstance()
                .collection(collectionPath)
                .document(documentPath)
                .set(value)
        }

    }
}

class ServerTimestamp(val timestamp: Long) {
    constructor(): this(0)
}