import { Base, IBase } from "./Base";
import { Player } from "./Player";

export interface ITeam extends IBase {
    _id: string,
    playerOne: Player,
    playerTwo: Player,
    score: number
}

export class Team extends Base implements ITeam, IBase {
    constructor(
        public _id: string,
        public playerOne: Player,
        public playerTwo: Player,
        public score: number
    ){
        super()
    }
}