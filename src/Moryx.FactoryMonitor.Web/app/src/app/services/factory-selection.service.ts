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

  public factorySelected = signal<number | undefined>(undefined);
  public defaultFactory = signal<FactoryStateModel | undefined>(undefined);
  public factoryContent = signal<VisualizableItemModel[]>([]);

  public selectFactory(factoryId: number | undefined) {
    if (!factoryId) {
      return;
    }

    //factory content, items to be displayed
    this.factoryMonitorService.factoryContent({factoryId: factoryId ?? 0})
      .then(items => {
        this.factoryContent.set(items);
        //manufacturing factory
        this.factorySelected.set(factoryId);
      });
  }

  public initialize(factory: FactoryStateModel) {
    this.defaultFactory.set(factory);
  }
}
