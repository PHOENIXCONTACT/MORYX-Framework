/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { FactoryStateModel } from '@api/models/factory-state-model';
import { VisualizableItemModel } from '@api/models/visualizable-item-model';
import { FactoryMonitorService } from '@api/services';

// ToDo: Make this a route resolver, it loads data and does not need to be a service for that.
@Injectable({
  providedIn: 'root'
})
export class FactorySelectionService {
  private factoryMonitorService = inject(FactoryMonitorService);

  private readonly _factorySelected = signal<number | undefined>(undefined);
  readonly factorySelected = this._factorySelected.asReadonly();
  private readonly _defaultFactory = signal<FactoryStateModel | undefined>(undefined);
  readonly defaultFactory = this._defaultFactory.asReadonly();
  private readonly _factoryContent = signal<VisualizableItemModel[]>([]);
  readonly factoryContent = this._factoryContent.asReadonly();

  public selectFactory(factoryId: number | undefined) {
    if (!factoryId) {
      return;
    }

    //factory content, items to be displayed
    this.factoryMonitorService.factoryContent({factoryId: factoryId ?? 0})
      .then(items => {
        this._factoryContent.set(items);
        //manufacturing factory
        this._factorySelected.set(factoryId);
      });
  }

  public initialize(factory: FactoryStateModel) {
    this._defaultFactory.set(factory);
  }
}
