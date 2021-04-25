import { IGameContext } from "../Messaging";

export default {
    pretentious: [
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} tu es mieux que ça! Mais voici l'indice demandé: ${ctx.hint}`, 
            en: `@${ctx.partner.username} you are better than this! But here is the hint: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} sérieusement? Moi je serais capable de deviner... Ton indice: ${ctx.hint}`, 
            en: `@${ctx.partner.username} seriously? I would be able to guess this... Your hint: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} c'est facile!! Mais le voici: ${ctx.hint}`, 
            en: `@${ctx.partner.username} it's easy!! But here is the hint: ${ctx.hint}`
        }),
    ],
    choleric: [
        (ctx: IGameContext) => ({   
            fr: `Frenchement @${ctx.partner.username}, t'as vraiment besoin d'un indice??? Le voici: ${ctx.hint}`, 
            en: `Come on @${ctx.partner.username}, you really need a hint??? Here it is: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} peux-tu vraiment être plus nulle? T'es mieux de ne pas en avoir beosin d'un autre! Indice: ${ctx.hint}`, 
            en: `@${ctx.partner.username} can you be worse than this? You better not need another one! Hint: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} Le voici: ${ctx.hint}. N'en demande pas d'autre!`, 
            en: `@${ctx.partner.username} Here: ${ctx.hint}. Don't ask for another one!`
        }),
    ], 
    joker: [
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} voici ton indice: ${ctx.hint}`, 
            en: `@${ctx.partner.username} here is your hint: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `@${ctx.partner.username} finalement je ne suis pas Picaso! Voici ton indice: ${ctx.hint}`, 
            en: `@${ctx.partner.username} finally I am not Picaso! Your hint: ${ctx.hint}`
        }),
        (ctx: IGameContext) => ({   
            fr: `Tu as raison @${ctx.partner.username}, Je ne sais pas c'est quoi ce spagetti... Voici l'indice: ${ctx.hint}`, 
            en: `You are right @${ctx.partner.username}, I don't understand either what this spagetti is... Here is the hint: ${ctx.hint}`
        }),
    ]
}