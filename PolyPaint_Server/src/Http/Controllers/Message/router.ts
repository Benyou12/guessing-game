import * as controller from './controller'

export default {
	prefix: '/message',
        routes: [
            {
                method: 'GET',
                path: '/',
                handlers: [
                    controller.getAll
                ]
            },
            {
                method: 'POST',
                path: '/',
                handlers: [
                    controller.create
                ]
            }
    ]
}