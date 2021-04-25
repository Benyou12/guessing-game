import * as controller from './controller'

export default {
	prefix: '/user',
        routes: [
            {
                method: 'GET',
                path: '/:uid',
                handlers: [
                    controller.get
                ]
            },
            {
                method: 'GET',
                path: '/conversation',
                handlers: [
                    controller.getInConversation
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
                path: '/:uid',
                handlers: [
                    controller.update
                ]
            },
            {
                method: 'POST',
                path: '/:uid/gamification',
                handlers: [
                    controller.gamification
                ]
            },
    ]
}

