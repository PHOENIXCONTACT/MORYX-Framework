/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { FactoryMonitorService } from '../api/services';
import { Subject } from 'rxjs';
import { CellSettingsModel } from '../api/models/cell-settings-model';
import { TranslateService } from '@ngx-translate/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { CellStoreService } from './cell-store.service';

@Injectable({
  providedIn: 'root'
})
export class CellSettingsService {
  private _cellSettingsChanged = new Subject<{ cellId: number; cellSettings: CellSettingsModel }>();

  public cellSettingsChanged$ = this._cellSettingsChanged.asObservable();

  constructor(
    private factoryMonitorService: FactoryMonitorService,
    public translate: TranslateService,
    public snackbarService: SnackbarService,
    private cellStoreService: CellStoreService) {

  }

  changeCellSettings(cellId: number, cellSettings: CellSettingsModel) {
    this.factoryMonitorService
      .cellSettings({
        id: cellId,
        body: cellSettings
      })
      .subscribe({
        next: _ => {
          this._cellSettingsChanged.next({ cellId, cellSettings });
          const cell = this.cellStoreService.getCell(cellId);
          if (!cell) return;
          cell.iconName = cellSettings.icon ?? 'build';
          cell.image = cellSettings.image ?? '';
          this.cellStoreService.updateCell(cell);
        },
        error: err => this.snackbarService.handleError(err)
      });
  }
}


