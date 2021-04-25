import * as uuid from "uuid/v4"
const jimp = require('jimp')
import * as fs from "fs"
import * as path from "path"
var admin = require("firebase-admin");

var serviceAccount = require("./PolyChat-cdc8fa9f10c6.json");

admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    storageBucket: "polychat-9c90e.appspot.com"
});

export const ResizeImg = async function (url: string) {

    console.log("URL FILE", url)

    const filename = `img-${Date.now()}.jpg`
    
    const filePath = path.resolve(__dirname, `../../../../temp/${filename}`) 

    console.log("PATH", filePath)

    const image = await jimp.read(url);
	await image.resize(250, jimp.AUTO);
    await image.writeAsync(filePath);

    const newLink = await UploadFile('image/jpg', `${filename}`, filePath, 'resized')

    // fs.unlinkSync(filePath)
    
    return newLink
}



export const UploadFile = async function (type, filename, filePath, folder = "default") {

    const newFilename: string = `${folder}/${uuid()}.${filename.split('.').pop()}`

    var bucket = await admin.storage().bucket().upload(filePath, {
        gzip: true,
        destination: newFilename,
        metadata: {
            cacheControl: 'public, max-age=31536000',
            contentType: type
        },
    });

    await admin.storage()
        .bucket()
        .file(newFilename)
        .makePublic();

    return `https://firebasestorage.googleapis.com/v0/b/polychat-9c90e.appspot.com/o/${encodeURIComponent(newFilename)}?alt=media`
}