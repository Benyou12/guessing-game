import { Cursor } from "mongodb"

export const DocumentsToArray = function<T> (documents: any, converter: Function): Array<T> {
	// console.log("DOCUMENTS", documents)
	let arr: Array<T> = []
	documents.forEach(doc => arr.push(converter(doc))) 
	return arr
}