export interface JoinRoom {
	uid: string
}

export interface Action {
	route: string,
	payload: object
}

export interface SocketRoute {
	action: Function,
	rooms: Function,
	response: {
		route: string,
		payload: Function
	}
}

export interface SocketContext<T> {
	uid?: string,
	request?: {
		body: object,
	},
	body?: T,
	uids?: string[]
	throw?: Function
}

export interface SocketError {
	code: number,
	message: string
}

export interface SocketResponse {
	route: string,
	rooms: string[],
	payload: object
}