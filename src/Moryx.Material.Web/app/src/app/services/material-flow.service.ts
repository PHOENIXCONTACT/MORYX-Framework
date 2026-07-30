import { inject, Injectable } from '@angular/core';
import { MaterialContainer } from '../models/material-container';
import { BehaviorSubject } from 'rxjs';
import { MaterialManagementService } from '../api/services';
import { ResourceModel } from '../api/models';

@Injectable({
  providedIn: 'root',
})
export class MaterialFlowService {
  private _filter = new BehaviorSubject<string[]>([]);
  $filter = this._filter.asObservable();
  private _containerAdded = new BehaviorSubject<ResourceModel | undefined>(undefined);
  $onContainerAdded = this._containerAdded.asObservable();

  constructor() {
  }

  executeFilter(filter: string[]) {
    this._filter.next(filter);
  }

  raiseContainerAdded(container: ResourceModel) {
    this._containerAdded.next(container);
  }

}