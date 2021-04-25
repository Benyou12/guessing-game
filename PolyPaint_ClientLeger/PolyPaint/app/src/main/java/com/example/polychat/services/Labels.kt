package com.example.polychat.services

import com.example.polychat.models.Label
import com.example.polychat.services.util.LangCode
import com.example.polychat.services.util.Localization

class Labels{
    companion object {
        val NAV_BAR_HOME = Label(en = "Home", fr = "Acceuil")
        val NAV_BAR_MESSENGER = Label(en = "Messenger", fr = "Messagerie")
        val NAV_BAR_GAME = Label(en = "Games", fr = "Jeux")
        val NAV_BAR_SIGN_OUT = Label(en = "SignOut", fr = "Déconnexion")
        val NAV_BAR_USERS = Label(en = "Users", fr = "Utilisateurs")
        val BUTTON_GAME_CLASSIC = Label(en = "Game | Classic Mode", fr = "Jeu | Mode classique")
        val PLAYED_LABEL = Label(en = "Played", fr = "Jouée" )
        val WIN_LABEL = Label(en = "Rounds Won", fr = "Partie Gagner" )
        val BADGES_LABEL = Label(en = "Badges", fr = "Jouée" )
        val USER_LABEL = Label(en = "Users", fr = "Utilisateurs" )
        val LOGED_IN = Label(en = "Logged in", fr = "Connecté")
        val LOGED_OUT = Label(en = "Logged out", fr = "Deconnecté")
        val SCOREBOARD = Label(en = "Scoreboard", fr = "Tableau de Bord")
        val SCORE = Label(en = "score", fr = "points")
        val ROUND_MESSAGE = fun(roundNumber: Int): String {
            return if(Localization.langCode == LangCode.EN) "Get Ready. Round $roundNumber is about to start"
            else "Soit prêt. Le tour $roundNumber est sur le point de commencer" }
        val WIN_MESSAGE = Label(en = "congratulations \uD83D\uDE00, you Won", fr = "Félicitations \uD83D\uDE00, vous avez gagné")
        val LOST_MESSAGE = Label(en = "You did your best \uD83D\uDE14 . Keep trying", fr = "Tu as fait de ton mieux \uD83D\uDE14. Continue d'essayer.")
        val DRAW_MESSAGE = Label(en = "Tie game \uD83E\uDD15. It was tight ", fr = "Match null \uD83E\uDD15. Ouf, C'etait serré")
        val END_MESSAGE = fun(isWin: Boolean?): String {
            return when (isWin)
            {
                true -> WIN_MESSAGE.getValue()
                false -> LOST_MESSAGE.getValue()
                null -> DRAW_MESSAGE.getValue()
            }
        }
        val LOBBY_MESSAGE = Label(en= "To start a game, you need four players. You Ask your friends to join first and add virtual players",
                fr = "Pour commencer une partie, vous avez besoin de quatre joueurs. Vous pouvez demander à vos amis de s’inscrire d’abord et d’ajouter des joueurs virtuels.")

        val LOG_OUT_MESSAGE = Label(en = "You've been disconneted. This user just signed in in an other device",
                fr = "Vous avez été déconnecté. Cet utilisateur vient de se connecter à un autre appareil")

        val WORD_TO_GUESS_VIEW_HINT = Label(en = "Enter your guess in ", fr = "Saisir un mot en ")
        val WORD_TO_GUESS_ANGLAIS= Label(en = "English ", fr = "Anglais ")
        val WORD_TO_GUESS_FRANCAIS= Label(en = "French ", fr = "Français ")
        val TUTORIAL_BTN = Label(
                en = "Tutorial",
                fr = "Tutoriel")
        val BACK_BUTTON_DIALOG_TITLE = Label(en = "Good to know!", fr ="Bon à savoir!")
        val BACK_BUTTON_DIALOG_MSG_= Label(en = "Use the navbar on top to navigate", fr ="Utiliser la navbar en haut pour naviguer")
    }
}