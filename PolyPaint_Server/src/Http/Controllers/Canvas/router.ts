import * as controller from './controller'

export default {
	prefix: '/game',
        routes: [
            {
                method: 'GET',
                path: '/:_id',
                handlers: [
                    controller.get
                ]
            },
            {
                method: 'POST',
                path: '/',
                handlers: [
                    controller.create
                ]
            },
            {
                method: 'PATCH',
                path: '/:_id/stroke',
                handlers: [
                    controller.updateStrokes
                ]
            },
    ]
}