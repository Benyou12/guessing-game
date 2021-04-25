import { ITeam } from "../Models/Team";
import { IAction } from "../Models/Action";
import { IRound } from "../Models/Round";
import { ICanvas } from "../Models/Canvas";
import { PERSONALITY } from "../Models/VirtualPlayer";

export interface IGetVirtualPlayer {
    user_id: string
}

export interface ICreateVirtualPlayer {
    user_id: string,
    personality: PERSONALITY,
    messages_used: string[]
}

export interface IUpdateVirtualPlayer {
    _id: string,
    user_id?: string,
    personality?: PERSONALITY,
    messages_used?: string[]
}
