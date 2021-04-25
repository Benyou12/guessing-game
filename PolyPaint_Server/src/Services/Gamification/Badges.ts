import { Badge } from "../../Models/Badge";

const badgeImg = (badge) => `https://firebasestorage.googleapis.com/v0/b/polychat-9c90e.appspot.com/o/badges%2F${badge}@2x.png?alt=media`

export const FirstGame = new Badge(
    "frist_game",
    "Première partie",
    "First Game",
    badgeImg("first_game_fr"),
    badgeImg("first_game_en"),
    "Vous avez complété une première partie!",
    "You completed your first game!",
    5
)

export const Plus10Games = new Badge(
    "plus_10_games",
    "+10 parties complétées",
    "+10 games completed",
    badgeImg("10_game_fr"),
    badgeImg("10_game_en"),
    "Vous avez complété 10 nouvelles parties!",
    "You completed 10 new games!",
    15
)

export const Plus1Hour = new Badge(
    "plus_1_hour",
    "+1 heure complétée",
    "+1 hour played",
    badgeImg("1_hour_fr"),
    badgeImg("1_hour_en"),
    "Vous avez joué pour une autre heure!",
    "You played for another hour!",
    25
)

export const AddVirtalPlayer = new Badge(
    "added_virtual_player",
    "Joueur virtuel ajouté",
    "Added a virtual player",
    badgeImg("virtual_player_fr"),
    badgeImg("virtual_player_en"),
    "Vous avez ajouté un joueur virtuel à une partie",
    "You added a virtual player to the game!",
    1
)