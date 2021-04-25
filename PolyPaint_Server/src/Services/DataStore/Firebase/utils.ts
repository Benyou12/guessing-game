import firebase from "./Firebase"

export const DocumentsToArray = function<T> (documents: firebase.firestore.QuerySnapshot, converter: Function): Array<T> {
	const arr: Array<T> = []
	documents.forEach(doc => arr.push({ id: doc.id, ... converter(doc.data()) }))
	return arr
}