import { IGameContext } from "../Messaging";

export default {
    pretentious: [
        () => ({   
            fr: "@{{partner.username}} c'était clair qu'on allait gagner!", 
            en: "@{{partner.username}} it was clear the we were going to win!"
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.victories) {
                return {   
                    fr: "On est en route pour notre {{partner.games_in_commun.victories}}ième victoire @{{partner.username}}!", 
                    en: "We are on our way to our {{partner.games_in_commun.victories}}th victory @{{partner.username}}!"
                }
            } else {
                return {   
                    fr: "On est en route pour notre première victoire @{{partner.username}}!", 
                    en: "We are on our way to our first victory @{{partner.username}}!"
                }
            }
        },
    ],
    choleric: [
        () => ({   
            fr: "@{{partner.username}} au moins on gagné cette round...", 
            en: "@{{partner.username}} at least we won this round..."
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.victories) {
                return {   
                    fr: "@{{partner.username}} Peut-être, juste peut-être, on va pouvoir avoir {{partner.games_in_commun.victories}}ième victoire", 
                    en: "@{{partner.username}} Maybe, just maybe, we could have our {{partner.games_in_commun.victories}}th victory"
                }
            } else {
                return {   
                    fr: "@{{partner.username}} Peut-être, juste peut-être, on va pouvoir avoir première victoire ensemble", 
                    en: "@{{partner.username}} Maybe, just maybe, we could have our first victory together"
                }
            }
        },
    ], 
    joker: [
        () => ({   
            fr: "Génial! @{{parter.username}} continuons comme ça!!!", 
            en: "Great! @{{parter.username}} let's continue like this!!!"
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.victories) {
                return {   
                    fr: "Encore en route pour une victoire ensemble!? On en a déjà {{partner.games_in_commun.victories}}!", 
                    en: "On the way to another victory together!? We already have {{partner.games_in_commun.victories}}!"
                }
            } else {
                return {   
                    fr: "Encore en route vers notre première une victoire ensemble!?", 
                    en: "On the way to our first victory together!?"
                }
            }
        },
    ]
}