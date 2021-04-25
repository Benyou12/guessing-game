import { MongoClientCommonOption, MongoCallback, MongoClient, Db, Collection } from "mongodb";

//const MongoClient = require('mongodb').MongoClient;
const uri = "mongodb+srv://admin:D4A0XD3E6StYJOK1@log3900-xmrmm.mongodb.net/test?retryWrites=true&w=majority";
const client: MongoClient = new MongoClient(uri, { useNewUrlParser: true, useUnifiedTopology: true });

var db: Db = null;

export const InitDB = function(): Promise<void> {
    return new Promise((resolve, reject) => {
        client.connect((err, database) => {
            if (err) reject(err)

            db = database.db("log3900")

            console.log("Youpi! Connected to mongo DB")
            resolve()
        });
    })
}

export const mongo = (collection: string): Collection => db.collection(collection)