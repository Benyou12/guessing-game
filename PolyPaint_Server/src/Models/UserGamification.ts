import { Base, IBase } from "./Base";
import { IUserBadge } from "./Badge";
import { ILevel } from "./Level";
import { Levels } from "../Services/Gamification/Levels"

export interface IUserGamification {
    badges: IUserBadge[],
    level: ILevel,
    points: number
}

export class UserGamification implements IUserGamification {
    constructor(
        public badges: IUserBadge[] = [],
        public level: ILevel = Levels[0],
        public points: number = 0
    ) { }
}