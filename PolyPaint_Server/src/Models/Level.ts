
export interface ILevel {
    number: number,
    name_fr: string,
    name_en: string,
    img: string,
    min_points: number
}

export class Level {

    constructor(
        public number: number,
        public name_fr: string,
        public name_en: string,
        public img: string,
        public min_points: number
    ) {}

}