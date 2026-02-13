/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FactoryStateStreamService } from './factory-state-stream.service';
import { OrderStoreService } from './order-store.service';
import { FactoryMonitorService } from '../api/services';
import { CellLocationModel } from '../api/models/cell-location-model';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { Converter } from '../extensions/converter';
import { FactoryStateModel } from '../api/models/factory-state-model';
import { ResourceChangedModel } from '../api/models/resource-changed-model';
import { ActivityChangedModel } from '../api/models/activity-changed-model';
import { CellStateChangedModel } from '../api/models/cell-state-changed-model';
import { FactorySelectionService } from './factory-selection.service';

@Injectable({
  providedIn: 'root'
})
export class CellStoreService {
  private _cellSelected = new BehaviorSubject<CellModel | undefined>(undefined);
  private _cellUpdated = new BehaviorSubject<CellModel | undefined>(undefined);
  private _cells = new BehaviorSubject<CellModel[]>([]);

  public cellSelected$ = this._cellSelected.asObservable();
  public cellUpdated$ = this._cellUpdated.asObservable();
  public cells$ = this._cells.asObservable();

  updatedCell: BehaviorSubject<CellModel | undefined> = new BehaviorSubject<CellModel | undefined>(undefined);

  constructor(
    private orderService: OrderStoreService,
    private factoryStateStreamService: FactoryStateStreamService,
    private factoryMonitorService: FactoryMonitorService,
    private factorySelectionService: FactorySelectionService,
    private snackbarService: SnackbarService
  ) {
    this.factoryMonitorService.initialFactoryState().subscribe({
      next: factoryState => {
        //only set default
        this.factorySelectionService.setDefaultFactory(factoryState);
        this.initializeOrders(factoryState);

        // get all the cells in the database that need to be displayed (only once), we will rely on the eventStream to update the list
        this.factoryMonitorService.allCells().subscribe(rawCells => {
          const rawRecourceChanges: Array<ResourceChangedModel> = [];

          rawCells.forEach(r =>
            rawRecourceChanges.push(<ResourceChangedModel>{
              id: r.id,
              cellIconName: r.iconName,
              cellLocation: r.location,
              factoryId: 0
            })
          );

          const rawActivityChanges: Array<ActivityChangedModel> = [];
          factoryState.activityChangedModels?.forEach(a => rawActivityChanges.push(a));
          let activityLength = rawActivityChanges.length;

          const rawCellStateChanges: Array<CellStateChangedModel> = [];
          factoryState.cellStateChangedModels?.forEach(c => rawCellStateChanges.push(c));

          let cells: { [id: string]: CellModel } = {};

          for (let raw of rawRecourceChanges) {
            let cell = Converter.resourceChangedModelToCell(raw);
            cell = Converter.addStateDataToCell(cell, raw);
            if (cell.id) cells[cell.id] = cell;
          }

          for (let i = 0; i < activityLength; i++) {
            let activity = rawActivityChanges[i];
            if (!activity.resourceId) continue;
            Converter.addActivityChangedModelToCell(cells[activity.resourceId], activity);
          }

          this._cells.next(Object.values(cells));
          this.subscribe();

          for (const key in cells) {
            this.updateCell(cells[key]);
          }
        });
      },
      error: err => this.snackbarService.handleError(err)
    });
  }

  private initializeOrders(factoryState: FactoryStateModel) {
    let orders: Order[] = [];
    let orderModels = factoryState.orderModels ?? [];

    orders = orderModels.map(order => Converter.orderModelToOrder(order));

    this.orderService._orders.next(orders);
    this.orderService.updateRunningOrders();
  }

  private subscribe() {
    this.factoryStateStreamService.updatedCell.subscribe({
      next: cell => {
        if (!cell?.id) return;
        if (!this._cells.getValue().length) return;

        this.updateCell(cell);
      }
    });
  }

  public selectCell(id: number | undefined) {
    if (this._cellSelected.getValue() && this._cellSelected.getValue()?.id === id || !id) {
      this._cellSelected.next(undefined);
      return;
    }

    const selectedCell = this._cells.getValue().find(c => c.id === id);
    this._cellSelected.next(selectedCell);
  }

  public moveCell(e: CellLocationModel) {
    //send cell update to server
    this.factoryMonitorService.moveCell({ body: e }).subscribe({
      error: err => this.snackbarService.handleError(err)
    });
  }

  public getCell(cellId: number) {
    return this._cells.getValue().find(c => c.id === cellId);
  }

  public updateCell(cell: CellModel) {
    const cells = this._cells.getValue();
    const indexToUpdate = cells.findIndex(x => x.id === cell.id);
    let cellToUpdate = cells[indexToUpdate];

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
    if (
      cellToUpdate.orderNumber &&
      cell.orderNumber != '' &&
      cellToUpdate.operationNumber &&
      cell.operationNumber != ''
    ) {
      cellToUpdate.orderColor =
        this.orderService._orders
          .getValue()
          .find(o => o.operationNumber === cellToUpdate.operationNumber && o.orderNumber === cellToUpdate.orderNumber)
          ?.orderColor ?? '';
    }

    cells[indexToUpdate] = cellToUpdate;
    this._cells.next(cells);
    this._cellUpdated.next(cellToUpdate);
  }
}

