import { IGameContext } from "../Messaging";

export default {
    pretentious: {
        requestHint: (ctx: IGameContext) => ({   
            fr: `Je vais dessiner! @${ctx.partner.username} écrit "J'ai besoin d'un indice!" si t'as besoin d'aide, mais pourquoi t'aurait besoin de ça?`, 
            en: `I will draw! @${ctx.partner.username} write "I need a hint!" if you need help guessing, but why would you need that?`
        }),
        noMoreHint: (ctx: IGameContext) => ({   
            fr: `Moi j'aurais deviné avec autant d'indices... @${ctx.partner.username}. J'en ai plus...`, 
            en: `I would have guessed with so many hints... @${ctx.partner.username}. I don't have hints left...`
        }),
    },
    choleric: {
        requestHint: (ctx: IGameContext) => ({   
            fr: `C'est à moi... @${ctx.partner.username} J'espère que tu ne vas pas l'utiliser, mais tu peux m'écrire "J'ai besoin d'aide".`, 
            en: `I will draw! @${ctx.partner.username} write "I need a hint!" if you need help guessing, but why would you need that?`
        }),
        noMoreHint: (ctx: IGameContext) => ({   
            fr: `T'es nulle @${ctx.partner.username}. T'as épuisé tout mes indices...`, 
            en: `You suck @${ctx.partner.username}. You used all my hints...`
        }),
    }, 
    joker: {
        requestHint: (ctx: IGameContext) => ({   
            fr: `Je dessine presque aussi bien que Picaso! Ou pas... Si t'as besoin @${ctx.partner.username}, envoie "Aide-moi!"`, 
            en: `I draw almost as well as Picaso! Or not... If you need it @${ctx.partner.username}, send "Help-me!"`
        }),
        noMoreHint: (ctx: IGameContext) => ({   
            fr: `Oh là là tu m'en demande trop @${ctx.partner.username}, je n'ai plus d'indices.`, 
            en: `Oh là là you are asking me to much @${ctx.partner.username}, I don't any hints left."`
        }),
    }
}