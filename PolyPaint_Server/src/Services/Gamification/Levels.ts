import { Level } from "../../Models/Level";

const levelImgLink = (level) => `https://firebasestorage.googleapis.com/v0/b/polychat-9c90e.appspot.com/o/levels%2F${level}@2x.png?alt=media`

export const Levels =  [
    new Level(1, "Nouveau", "Newbie", levelImgLink("level_1"), 0),
    new Level(2, "Débutant", "Rookie", levelImgLink("level_2"), 30),
    new Level(3, "Avancé", "Advanced", levelImgLink("level_3"), 60),
    new Level(4, "Expert", "Expert", levelImgLink("level_4"), 120),
    new Level(5, "Maître", "Master", levelImgLink("level_5"), 240),
]

