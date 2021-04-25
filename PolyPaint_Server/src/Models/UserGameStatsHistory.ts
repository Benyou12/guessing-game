import { Base, IBase } from "./Base";

export interface IUserGameStatsHistory extends IBase {
    game_id: string,
    timestamp: number,
    names: string[],
    myTeamResult: number,
    otherTeamResult: number,
    name: string
}

export class UserGameStatsHistory extends Base implements IUserGameStatsHistory, IBase {
    constructor(
        public game_id: string,
        public timestamp: number,
        public names: string[],
        public myTeamResult: number,
        public otherTeamResult: number,
        public name: string
    ) {
        super()
    }
}