import { Base, IBase } from "./Base";

export interface IUserGameStats extends IBase {
    rounds_played: number
    victories: number
    failures: number
    rounds_avg_time: number
    total_time_played: number,
    total_games_played: number
}

export class UserGameStats extends Base implements IUserGameStats, IBase {
    constructor(
        public rounds_played: number = 0,
        public victories: number = 0,
        public failures: number = 0,
        public rounds_avg_time: number = 0,
        public total_time_played: number = 0,
        public total_games_played: number = 0
    ) {
        super()
    }
}