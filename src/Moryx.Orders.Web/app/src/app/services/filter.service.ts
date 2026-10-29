import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FilterService {
  private readonly hideCompletedStorageKey = 'operations-hide-completed';

  private readonly _hideCompleted = signal(this.loadHideCompleted());
  readonly hideCompleted = this._hideCompleted.asReadonly();

  private readonly _searchTerm = signal('');
  readonly searchTerm = this._searchTerm.asReadonly();

  toggleHideCompleted(): void {
    const newValue = !this.hideCompleted();
    window.localStorage.setItem(this.hideCompletedStorageKey, newValue.toString());
    this._hideCompleted.set(newValue);
  }

  setSearchTerm(term: string): void {
    this._searchTerm.set(term);
  }

  private loadHideCompleted(): boolean {
    const stored = window.localStorage.getItem(this.hideCompletedStorageKey);
    return stored === null ? true : stored === 'true';
  }
}
