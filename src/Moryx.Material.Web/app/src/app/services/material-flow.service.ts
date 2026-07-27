import { inject, Injectable } from '@angular/core';
import { MaterialContainer } from '../models/material-container';
import { BehaviorSubject } from 'rxjs';
import { MaterialManagementService } from '../api/services';

@Injectable({
  providedIn: 'root',
})
export class MaterialFlowService {
  private _filter = new BehaviorSubject<string[]>([]);
  $filter = this._filter.asObservable();
  
  constructor() {
  }

  executeFilter(filter: string[] ) {
    this._filter.next(filter);
  }

}