import * as controller from './controller'

export default {
	prefix: '/time',
        routes: [
            {
                method: 'GET',
                path: '/now',
                handlers: [
                    controller.getTimestamp
                ]
            }
    ]
}

