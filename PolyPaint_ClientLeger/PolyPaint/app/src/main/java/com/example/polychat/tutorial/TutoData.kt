package com.example.polychat.tutorial

import com.example.polychat.models.Label

val msg1 = Label(
        en = "You can join a group (less than 4 players) to start the game.",
        fr = "Vous pouvez rejoindre un groupe (moins de 4 joueurs) pour commencer la partie.")
val msg2 = Label(
        en = "While waiting to complete the number of players (4 players) needed to start the game, the “Start Game” button is disabled. Once the number of the group is 4, press the “Start Game” button to have the proposed team division.",
        fr = "En attendant de compléter le nombre de joueurs (4 joueurs)  nécessaire pour commencer la partie, le bouton de “Commencer la partie” est désactivé. Une fois le nombre du groupe est 4, appuyez sur le bouton “Commencer la partie” pour avoir la division d’équipe proposée.")
val msg3 = Label(
        en = "Press the “Start Game” button, the app will divide the group into two teams.",
        fr = "Appuyez  sur le bouton “Commencer la partie”, l’application divisera le groupe en deux équipes.")
val msg4 = Label(
        en = "In your turn, a member of your team is allowed to draw the proposed word at the top to draw it down in the canvas.",
        fr = "À votre tour, un membre de votre équipe est autorisé à dessiner le mot proposé en haut pour le dessiner en bas dans le canvas.")
val msg5 = Label(
        en = "The other member of your team tries to guess the word by sending the answer.",
        fr = "L'autre membre de votre équipe essaye de deviner le mot en envoyant la réponse.")
val msg6 = Label(
        en = "If the answer is incorrect, the opposing team may try to guess in turn.",
        fr = "Si la réponse est incorrect, l'équipe adversaire peut essayer de deviner à son tour.")
val msg7 = Label(
        en = "The team with the highest score by guessing the maximum number of words will win.",
        fr = "L'équipe qui aura le meilleur score en devinant le maximum de mots gagnera.")

val title1 = Label(en = "Join a group", fr = "Rejoindre un groupe")

val title2 = Label(en = "Waiting", fr = "En attente de joueurs")

val title3 = Label(en = "Start the game", fr = "Début de la partie")

val title4 = Label(en = "Your team draws", fr = "Votre équipe dessine")

val title5 = Label(en = "Your teammate guesses the drawing", fr = "Votre coéquipier devine le dessin")

val title6 = Label(en = "The right of reply", fr = "Le droit de réplique")

val title7 = Label(en = "Win the game", fr = "Gagner la partie")