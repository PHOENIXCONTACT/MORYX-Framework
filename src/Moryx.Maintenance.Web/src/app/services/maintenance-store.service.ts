import { computed, Injectable, signal } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceStoreService {

  public readonly currentPageLabel = signal<string | undefined>(undefined);


  constructor() { }

  setPageLabel(value: string){
    this.currentPageLabel.set(value);
  }
}

