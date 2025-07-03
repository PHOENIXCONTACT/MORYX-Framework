import { Injectable, signal } from '@angular/core';

const FULLSCREEN_KEY = 'workerInstructions.fullscreenEnabled';

@Injectable({
  providedIn: 'root',
})
export class InstructionStateService {
  private fullscreen = signal<boolean>(this.loadInitialState());

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
    this.fullscreen.set(newValue);
    try {
      localStorage.setItem(FULLSCREEN_KEY, JSON.stringify(newValue));
    } catch {}
  }

  getFullscreenState() {
    return this.fullscreen.asReadonly();
  }
}
