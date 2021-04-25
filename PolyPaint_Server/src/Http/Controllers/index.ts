import * as glob from 'glob'
import * as Router from 'koa-router'
import { Context } from 'vm';

function getBody(ctx: Context) {
  return {
    ...ctx.params,
    ...ctx.request.query,
    ...ctx.request.body
  }
}


export default function (app: any) {
  glob(`${__dirname}/*`, { ignore: ['**/index.ts', '**/index.js'] }, (err: any, matches: any) => {
    if (err) {
      throw err
    }

    matches.forEach((mod: any) => {
      const router = require(`${mod}/router`)
      const { routes, prefix } = router.default
      const instance: any = new Router({ prefix })

      routes.forEach((config: any) => {
        const { method = '', path = '', handlers = [] } = config

        const lastHandler = handlers.pop() 

        instance[method.toLowerCase()](path, ...handlers, (ctx: Context) => lastHandler(ctx, getBody(ctx)))

        app.use(instance.routes()).use(instance.allowedMethods())

      })
    })
  })
}