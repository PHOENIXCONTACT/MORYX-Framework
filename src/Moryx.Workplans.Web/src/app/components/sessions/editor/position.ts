export class Position {
    /**
     *
     */
    constructor(public left: number, public top: number) {        
    }

    move(x: number, y: number) {
        this.left += x;
        this.top += y;
    }
}