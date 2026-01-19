/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';

const FULLSCREEN_KEY = 'workerInstructions.fullscreenEnabled';

@Injectable({
  providedIn: 'root',
})
export class InstructionStateService {
  private _fullscreen = signal<boolean>(this.loadInitialState());
  fullscreen = this._fullscreen.asReadonly();
  constructor() {}

  private loadInitialState(): boolean {
    try {
      const raw = localStorage.getItem(FULLSCREEN_KEY);
      return raw !== null ? JSON.parse(raw) : false;
    } catch {
      return true;
    }
  }

  toggleFullscreen(): void {
    const newValue = !this.fullscreen();
    this._fullscreen.set(newValue);
    try {
      localStorage.setItem(FULLSCREEN_KEY, JSON.stringify(newValue));
    } catch {}
  }

}

