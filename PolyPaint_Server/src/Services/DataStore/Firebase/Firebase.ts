import * as firebase from "firebase/app";
import "firebase/auth";
import "firebase/firestore";

const firebaseConfig = {
    apiKey: "AIzaSyAY_l7wnQRTNRmKHSCJPVnziM74hb8NziU",
    authDomain: "polychat-9c90e.firebaseapp.com",
    databaseURL: "https://polychat-9c90e.firebaseio.com",
    projectId: "polychat-9c90e",
    storageBucket: "polychat-9c90e.appspot.com",
    messagingSenderId: "880081131415",
    appId: "1:880081131415:web:6363bd6eb09f350a7816d2"
};

firebase.initializeApp(firebaseConfig);

export default firebase