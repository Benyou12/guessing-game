package com.example.polychat.models.socket

enum class SocketEvents (val event: String) {
    JOIN_ROOM("join-room"),
    ROOM_JOINED("room-joined"),
    PING("ping1"),
    PONG("pong1"),
    GET_CONVERSATIONS("conversation.get.all"),
    GET_CONVERSATION("conversation.get.one"),
    CREATE_CONVERSATION("conversation.post.create"),
    GET_CONVERSATION_MESSAGES("message.get.all"),
    CREATE_MESSAGE("message.post.create"),
    GET_CONVERSATION_USERS("user.get.all"),
    GET_USER_STATS("user.get.one"),
    CONERSATION_ALL("conversation.all"),
    CONVERSATION_ONE("conversation.one"),
    CONVERSATION_NEW("conversation.new"),
    CONVERSATION_ADD_USER("conversation.patch.addUser"),
    CONVERSATION_REMOVE_USER("conversation.patch.removeUser"),
    CONVERSATION_UPDATE("conversation.updated"),
    INVITE_USER("conversation.post.invite"),
    CONVERSATION_INVITE("conversation.invite"),
    MESSAGE_ALL("message.all"),
    MESSAGE_NEW("message.new"),
    USER_CONVERSATION("user.conversation"),
    GAT_ALL_USERS("user.get.all"),
    ALL_USERS("user.all"),
    CREATE_CANVAS("canvas.post.create"),
    CANVAS_CREATED("canvas.created"),
    SEND_STROKE("canvas.patch.stroke"),
    STROKE_UPDATED("canvas.stroke.updated"),
    GAME_ON("game.on"),
    SEARCH_REQUEST("conversation.get.search"),
    SEARCH_RESPONSE("conversation.search"),
    GET_USER_ONLINE("user.get.online"),
    USER_ONLINE("user.online"),
    USER_OAUTH("auth.oauth"),
    USER_ONE("user.one"),
    USER_LOG_OUT("auth.logout")
}

enum class GameEvent(val event: String){
    GET_ALL_GAME("game.get.all"),
    GET_ONE_GAMR("game.get.one"),
    GET_ACTIVE_GAME("game.get.active"),
    CREATE_GAME("game.post.create"),
    JOIN_GAME("game.patch.join"),
    UPDATE_GAME("game.patch.update"),
    ALL_GAME("game.all"),
    ACTIVE_GAME("game.active"),
    ONE_GAME("game.one"),
    CREATED_GAME("game.created"),
    GAME_UPDATED("game.updated"),
    ADD_VIRTUAL_PLAYER("game.patch.addVirtual"),
    GAME_PATCH_ACTION("game.patch.action"),
    GAME_QUIT("game.patch.quit"),
    GAME_CANCELED("game.canceled"),
    GAME_DELETE("game.delete"),
    GAME_DELETED("game.deleted")
}