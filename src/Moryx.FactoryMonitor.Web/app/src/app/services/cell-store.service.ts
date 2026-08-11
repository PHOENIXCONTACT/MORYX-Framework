/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

import { VisualizableItemModel } from '@api/models';
import { CellLocationModel } from '@api/models/cell-location-model';
import { FactoryStateModel } from '@api/models/factory-state-model';
import { FactoryMonitorService } from '@api/services';
import { Converter } from '../extensions/converter';
import CellModel from '../models/cellModel';
import { FactoryStateStreamService } from './factory-state-stream.service';
import { OrderStoreService } from './order-store.service';

@Injectable({
  providedIn: 'root'
})
export class CellStoreService {
  private readonly orderService = inject(OrderStoreService);
  private readonly factoryStateStreamService = inject(FactoryStateStreamService);
  private readonly factoryMonitorService = inject(FactoryMonitorService);
  private readonly snackbarService = inject(SnackbarService);

  private readonly _cellSelected = signal<CellModel | undefined>(undefined);
  readonly cellSelected = this._cellSelected.asReadonly();
  private readonly _cellUpdated = signal<CellModel | undefined>(undefined);
  readonly cellUpdated = this._cellUpdated.asReadonly();
  private _cells : CellModel[] = [];

  constructor() {
    effect(() => {
      const cell = this.factoryStateStreamService.updatedCell();
      if (cell) {
        this.updateCell(cell);
      }
    });
  }

  public initialize(factoryState: FactoryStateModel) {
    const cells: { [id: string]: CellModel; } = {};
    const initialRecourceChanges = factoryState.resourceChangedModels ?? [];
    for (const raw of initialRecourceChanges) {
      const cell = Converter.resourceChangedModelToCell(raw);
      if (cell.id) {
        cells[cell.id] = cell;
      }
    }

    const initialStateChanges = factoryState.cellStateChangedModels ?? [];
    for (const raw of initialStateChanges) {
      if (!raw.id) {
        continue;
      }
      const cell = cells[raw.id];
      Converter.addStateDataToCell(cell, raw);
    }

    const initialActivityChanges = factoryState.activityChangedModels ?? [];
    for (const raw of initialActivityChanges) {
      if (!raw.resourceId) {
        continue;
      }
      const cell = cells[raw.resourceId];
      Converter.addActivityChangedModelToCell(cell, raw);
      this.orderService.applyOrderColor(cell);
    }

    // Set initial cells
    this._cells = Object.values(cells);

  }

  public selectCell(id: number | undefined) {
    const current = this.cellSelected();
    if (current && current.id === id || !id) {
      this._cellSelected.set(undefined);
      return;
    }

    const selectedCell = this._cells.find(c => c.id === id);
    this._cellSelected.set(selectedCell);
  }

  public async moveItem(item: VisualizableItemModel, update: CellLocationModel) {
    try {
      const location = await this.factoryMonitorService.moveCell({ body: update });
      this.updateCell(<CellModel>{ id: item.id, location: location });
    } catch (error) {
      this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }
  }

  public getCell(cellId: number) : CellModel {
    const cell = this._cells.find(c => c.id === cellId)
    if (!cell) {
      throw Error(`Tried to process unknown cell with id ${cellId}`);
    }
    return cell;
  }

  public getCells(factory: FactoryStateModel): CellModel[] {
    return this._cells.filter(c => c.factoryId === factory.id);
  }

  // Cell updates are retrieved as partial updates to the existing cell models,
  // so we need to merge the incoming data with the existing cell data
  public updateCell(cell: CellModel) {
    const indexToUpdate = this._cells.findIndex(x => x.id === cell.id);
    const cellToUpdate = {... this._cells[indexToUpdate]};

    if (cell.iconName != '' && cell.iconName) {
      cellToUpdate.iconName = cell.iconName;
    }
    if (cell.image != '' && cell.image) {
      cellToUpdate.image = cell.image;
    }
    if (cell.name != '' && cell.name) {
      cellToUpdate.name = cell.name;
    }
    if (cell.factoryId) {
      cellToUpdate.factoryId = cell.factoryId;
    }
    if (cell.propertySettings) {
      cellToUpdate.propertySettings = cell.propertySettings;
    }
    if (cell.state) {
      cellToUpdate.state = cell.state;
    }
    if (cell.classification) {
      cellToUpdate.classification = cell.classification;
    }
    if (cell.orderNumber) {
      cellToUpdate.orderNumber = cell.orderNumber;
    }
    if (cell.operationNumber) {
      cellToUpdate.operationNumber = cell.operationNumber;
    }
    if (cellToUpdate.orderNumber && cell.orderNumber && cellToUpdate.operationNumber &&
      cell.operationNumber) {
      this.orderService.applyOrderColor(cellToUpdate);
    }
    if (cell.location) {
      cellToUpdate.location = cell.location;
    }

    this._cells[indexToUpdate] = cellToUpdate;
    this._cellUpdated.set(cellToUpdate);
  }
}

