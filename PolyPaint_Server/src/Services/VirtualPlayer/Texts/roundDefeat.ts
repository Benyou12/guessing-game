import { IGameContext } from "../Messaging"
import * as moment from "moment"

export default {
    pretentious: [
        () => ({   
            fr: "C'était clairement pas de notre faute! Le serveur a-t-il eu un problème???", 
            en: "It clearly wasn't our fault! Did the server fail on us???"
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.failures) {
                return {   
                    fr: "On est en route pour notre {{partner.games_in_commun.failures}}ième victoire @{{partner.username}}!", 
                    en: "We are on our way to our {{partner.games_in_commun.failures}}th victory @{{partner.username}}!"
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
            fr: "{{partner.username}} vou me surprenez.", 
            en: "{{partner.username}} you suprise me."
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.failures) {
                return {   
                    fr: "Sérieusement @{{partner.username}}, nous avons déjà {{partner.games_in_commun.failures}} défaites ensemble...", 
                    en: "Come on @{{partner.username}}, we already have {{partner.games_in_commun.failures}} failures together..."
                }
            } else {
                return {   
                    fr: "Sérieusement @{{partner.username}}, déjà en route vers notre première défaite ensemble...", 
                    en: "Come on @{{partner.username}}, already on the way to our first failure together..."
                }
            }
        },
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.worstGame) {
                return {   
                    fr: `F*** @${ctx.partner.username}, si on continue comme ça, on va avoir ${ctx.partner.games_in_commun.worstGame} points comme notre pire partie d'il y a ${moment().lang('fr').to(ctx.partner.games_in_commun.worstGame.timestamp)}`, 
                    en: `F*** @${ctx.partner.username}, if we continue like this, we will have ${ctx.partner.games_in_commun.worstGame} points like our worst game of ${moment().lang('fr').to(ctx.partner.games_in_commun.worstGame.timestamp)}`
                }
            } else {
                return {   
                    fr: "F*** @{{partner.username}}, est-ce qu'on va perdre?? Belle première partie.", 
                    en: "F*** @{{partner.username}}, are we going to loose?? Nice first game."
                }
            }
        },
    ], 
    joker: [
        () => ({   
            fr: "{{parter.username}} ce n'est pas fini! Comme on dit, jamais 201?", 
            en: "{{parter.username}} it's not done! Like we say, never 201?"
        }),
        (ctx: IGameContext) => {
            if (ctx.partner.games_in_commun.failures) {
                return {   
                    fr: `@${ctx.partner.username} On n'a pas raté ce tour, on a juste rien eu de bon! Comme nos ${ctx.partner.games_in_commun.failures} autres parties ratées...`, 
                    en: `@${ctx.partner.username} We didn't fail this round, we only got everything wrong! Just like our ${ctx.partner.games_in_commun.failures} other failed games...`
                }
            } else {
                return {   
                    fr: `@${ctx.partner.username} On n'a pas raté ce tour, on a juste rien eu de bon!`, 
                    en: `@${ctx.partner.username} We didn't fail this round, we only got everything wrong!`
                }
            }
        },
    ]
}