import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FilterService {
  private readonly hideCompletedStorageKey = 'operations-hide-completed';
  private readonly hideCompletedSubject = new BehaviorSubject<boolean>(this.loadHideCompleted());

  readonly hideCompleted$ = this.hideCompletedSubject.asObservable();

  get hideCompleted(): boolean {
    return this.hideCompletedSubject.value;
  }

  toggleHideCompleted(): void {
    const newValue = !this.hideCompletedSubject.value;
    window.localStorage.setItem(this.hideCompletedStorageKey, newValue.toString());
    this.hideCompletedSubject.next(newValue);
  }

  private loadHideCompleted(): boolean {
    const stored = window.localStorage.getItem(this.hideCompletedStorageKey);
    return stored === null ? true : stored === 'true';
  }
}
