/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FactoryStateModel } from '@api/models/factory-state-model';
import { VisualizableItemModel } from '@api/models/visualizable-item-model';
import { FactoryMonitorService } from '@api/services';

// ToDo: Make this a route resolver, it loads data and does not need to be a service for that.
@Injectable({
  providedIn: 'root'
})
export class FactorySelectionService {
  private _selectedFactory = new BehaviorSubject<number|undefined>(undefined);
  private _defaultFactory = new BehaviorSubject<FactoryStateModel|undefined>(undefined);
  private _selectedFactoryContent = new BehaviorSubject<VisualizableItemModel[]>([]);

  private factoryMonitorService = inject(FactoryMonitorService);
  public factorySelected$ = this._selectedFactory.asObservable();
  public defaultFactory$ = this._defaultFactory.asObservable();
  public factoryContent$ = this._selectedFactoryContent.asObservable();

  public selectFactory(factoryId: number|undefined){

    if(!factoryId) return;

    //factory content, items to be displayed
    this.factoryMonitorService.factoryContent({factoryId: factoryId ?? 0})
      .subscribe(items => {
        this._selectedFactoryContent.next(items);
        //manufacturing factory
        this._selectedFactory.next(factoryId);
      });
  }

  public initialize(factory: FactoryStateModel){
    this._defaultFactory.next(factory);
  }
}

