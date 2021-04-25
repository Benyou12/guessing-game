import * as controller from './controller'

export default {
	prefix: '/auth',
        routes: [
            {
                /**
                 * Body: ILogin
                 * Returns: IUser
                 */
                method: 'POST',
                path: '/login',
                handlers: [
                    controller.login
                ]
            },
            {
                /**
                 * Body: ILogout
                 * Returns: ILogout
                 */
                method: 'POST',
                path: '/logout',
                handlers: [
                    controller.logout
                ]
            },

    ]
}