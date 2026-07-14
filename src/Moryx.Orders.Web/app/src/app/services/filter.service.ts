import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FilterService {
  private readonly hideCompletedStorageKey = 'operations-hide-completed';

  readonly hideCompleted = signal(this.loadHideCompleted());

  toggleHideCompleted(): void {
    const newValue = !this.hideCompleted();
    window.localStorage.setItem(this.hideCompletedStorageKey, newValue.toString());
    this.hideCompleted.set(newValue);
  }

  private loadHideCompleted(): boolean {
    const stored = window.localStorage.getItem(this.hideCompletedStorageKey);
    return stored === null ? true : stored === 'true';
  }
}
