
export interface IBadge {
    badge_id: string,
    name_fr: string,
    name_en: string,
    img_fr: string,
    img_en: string,
    desc_fr: string,
    desc_en: string,
    points: number
}

export class Badge {

    constructor(
        public badge_id: string,
        public name_fr: string,
        public name_en: string,
        public img_fr: string,
        public img_en: string,
        public desc_fr: string,
        public desc_en: string,
        public points: number
    ) {}
}

export interface IUserBadge {
    timestamp: number,
    game_id: string,
    badge: IBadge
}

export class UserBadge {

    constructor(
        public timestamp: number,
        public game_id: string,
        public badge: IBadge
    ) {}

}