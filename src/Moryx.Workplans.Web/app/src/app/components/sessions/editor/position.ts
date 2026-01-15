/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

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
