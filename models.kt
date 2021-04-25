data class User (
    val uid: string,
    val username: string,
    val firstname: string,
    val lastname: string,
    val email: string,
    val profileImgUrl: string,
    val isConnected: boolean,
    val stat: UserStat,
)
{

}

////////////////////////////////////
//////// Stats ////////////////////
//////////////////////////////////


data class UserStat (
    val numberPartiesPlayed: int,
    val victories: int,
    val losses: int,
    val averageGameTime: int,
    val totalTimePlayed: int,
    val loginActivities: LoginActivity[],
    val game: GameStats[],
    val badges: Badge[],
) 
{

}

data class LoginActivity (
    val timestamp: long,
    val isLogin: boolean, 
) {

}

data class GameStats (
    val users: User[],
    val score: int,
    val isVictory: boolean,
    val startTime: long,
    val endTime: long,
    val mode: GameMode,
    val numberOfHints: int?, 
) {

}

enum GameMode {
    classique   = 0,
    solo        = 1,
    coop        = 2,
}

data class Badge (
    val name: string,
    val desc: string,
    val logoUrl: string,
    val cost: int,
) {}
////////////////////////////////////
//////// Messages /////////////////
//////////////////////////////////
data class Conversation (
     val messages: Message[],
     val users: User[],
 ) {

 }

data class message (
    val fromUid: string, 
    val text: string,
    val timestamp: long,
) {

}

/**
-- Conversations --> 
        - conversation_1
            - messages 
                - message_1
                - message_2
                _ message_3
            - user 
        - conversation_2
            - message_1
            - message_2
            _ message_3 

        - conversation_3
            - message_1
            - message_2
            _ message_3 


 */