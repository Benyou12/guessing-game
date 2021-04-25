import { IGame } from "../../Models/Game";
import { IRound } from "../../Models/Round";
import { IStroke, ICanvas } from "../../Models/Canvas";
import { EmitInRoom } from "../../Socket";
import { getUids } from "../DataStore/Game";
import CanvasController from "../../Socket/Controllers/canvas"
import { IGameImage, GAME_DIFFICULTY, DRAWING_MODES } from "../../Models/GameImage";
const EventEmitter = require('events');

class DrawingEvent extends EventEmitter {}


export const drawingEvent = new DrawingEvent();

export default class VirtualDrawing {

    private game: IGame = null
    private strokeIndex: number = -1
    private gameImage: IGameImage = null
    private canvas: ICanvas = null
    private nbrOfPoints: number = 10
    private cancel: boolean = false

    constructor(game: IGame, gameImage: IGameImage) {
        this.game = game
        this.gameImage = gameImage
        this.canvas = gameImage.canvas

        if (this.gameImage.drawing_mode == DRAWING_MODES.RANDOM) {
            this.shuffleArray(this.canvas.strokes)
        }
    }

    public async startDrawing(delay: number) {

        this.listenCancel()

        await this.wait(delay)

        switch(this.gameImage.difficulty) {
            case GAME_DIFFICULTY.EASY:
                this.nbrOfPoints = 1
                break
            case GAME_DIFFICULTY.MEDIUM: 
                this.nbrOfPoints = 2
                break
            case GAME_DIFFICULTY.HARD:
                this.nbrOfPoints = 4
                break
        }

        await this.nextStroke()
    }

    public cancelDrawing() {
        this.cancel = true
    }

    private listenCancel() {
        console.log("LISTENING CANCEL", `drawingEvent-${this.gameImage._id}`)
        drawingEvent.on(`drawingEvent-${this.gameImage._id}`, () => {
            console.log("RECEIVED CANCEL", `drawingEvent-${this.gameImage._id}`)
            this.cancelDrawing()
        })
    }

    private async nextStroke() {
        //console.log("STROKES LENGYH", this.strokeIndex, this.canvas.strokes.length, this.cancel)
        if (this.strokeIndex < this.canvas.strokes.length - 1 && !this.cancel) {
            this.strokeIndex += 1
            await this.drawStroke(this.canvas.strokes[this.strokeIndex])
        }
            
    }

    private async drawStroke(stroke: IStroke) {

        //console.log("DRAW STROKE ", this.strokeIndex)
        let segments: number = Math.ceil(stroke.coordinates.length / this.nbrOfPoints)

        for (let i = 0; i < segments; i++) {    
            if (this.cancel) {
                break
            }

            let strokeCopy = {...stroke}
            strokeCopy.coordinates = stroke.coordinates.slice(this.nbrOfPoints*i, (this.nbrOfPoints*i)+this.nbrOfPoints)

            EmitInRoom(getUids(this.game), CanvasController.patch.stroke.response, {_test: i, ...strokeCopy})

            await this.wait(50)
        }

        await this.wait((3 - this.gameImage.difficulty) * 500 + 500)
        // console.log("NEXT STROKE")
        await this.nextStroke()
        
    }

    private async wait(duration: number) {
        return new Promise((resolve, reject) => {
            setTimeout(resolve, duration)
        })
    }

    private shuffleArray(array: IStroke[]): void {
        for (let i = array.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
    }

}