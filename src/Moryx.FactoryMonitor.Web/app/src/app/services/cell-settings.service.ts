/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { FactoryMonitorService } from '../api/services';
import { Subject } from 'rxjs';
import { CellSettingsModel } from '../api/models/cell-settings-model';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { CellStoreService } from './cell-store.service';
import CellModel from '../models/cellModel';

@Injectable({
  providedIn: 'root'
})
export class CellSettingsService {
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);
  private cellStoreService = inject(CellStoreService);


  changeCellSettings(cellId: number, cellSettings: CellSettingsModel) {
    this.factoryMonitorService
      .cellSettings({
        id: cellId,
        body: cellSettings
      })
      .subscribe({
        next: _ => {
          const cell = <CellModel>{
            id: cellId,
            iconName: cellSettings.icon,
            image: cellSettings.image
          }
          this.cellStoreService.updateCell(cell);
        },
        error: err => this.snackbarService.handleError(err)
      });
  }
}


