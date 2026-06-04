/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, lastValueFrom, ReplaySubject } from 'rxjs';
import { FactoryStateStreamService } from './factory-state-stream.service';
import { OrderStoreService } from './order-store.service';
import { FactoryMonitorService } from '../api/services';
import { CellLocationModel } from '../api/models/cell-location-model';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { Converter } from '../extensions/converter';
import { FactoryStateModel } from '../api/models/factory-state-model';
import { FactorySelectionService } from './factory-selection.service';
import { HttpErrorResponse } from '@angular/common/http';
import { VisualizableItemModel } from '../api/models';


// ToDo: While this is called cell-store service it actually holds all items 
// (also factories). 
@Injectable({
  providedIn: 'root'
})
export class CellStoreService {
  private _orderService = inject(OrderStoreService);
  private factoryStateStreamService = inject(FactoryStateStreamService);
  private factoryMonitorService = inject(FactoryMonitorService);
  private factorySelectionService = inject(FactorySelectionService);
  private snackbarService = inject(SnackbarService);

  private _cellSelected = new BehaviorSubject<CellModel | undefined>(undefined);
  private _cellUpdated = new ReplaySubject<CellModel>();
  private _cells : CellModel[] = [];

  public cellSelected$ = this._cellSelected.asObservable();
  public cellUpdated$ = this._cellUpdated.asObservable();

  updatedCell: BehaviorSubject<CellModel | undefined> = new BehaviorSubject<CellModel | undefined>(undefined);

  // ToDo: Move async work to an provideAppInitializer
  constructor() {
    this.init();
  }

  private async init() {
    let factoryState: FactoryStateModel | undefined;
    try {
      factoryState = await lastValueFrom(this.factoryMonitorService.initialFactoryState());
    } catch (error) {
      this.snackbarService.handleError(error as HttpErrorResponse);
    }

    if (!factoryState) {
      return;
    }

    this.factorySelectionService.setDefaultFactory(factoryState);
    // ToDo: Make method call on order service
    const orders = this.initializeOrders(factoryState);

    let cells: { [id: string]: CellModel; } = {};
    const initialRecourceChanges = factoryState.resourceChangedModels ?? [];
    for (let raw of initialRecourceChanges) {
      const cell = Converter.resourceChangedModelToCell(raw);
      if (cell.id)
        cells[cell.id] = cell;
    }

    const initialStateChanges = factoryState.cellStateChangedModels ?? [];
    for (let raw of initialStateChanges) {
      if (!raw.id) continue;
      const cell = cells[raw.id];
      Converter.addStateDataToCell(cell, raw);
    }

    const initialActivityChanges = factoryState.activityChangedModels ?? [];
    for (let raw of initialActivityChanges) {
      if (!raw.resourceId) continue;
      const cell = cells[raw.resourceId];
      Converter.addActivityChangedModelToCell(cell, raw);
      this._orderService.applyOrderColor(cell);
    }

    this._cells = Object.values(cells);
    this.subscribe();
  }

  private initializeOrders(factoryState: FactoryStateModel) {
    let orders: Order[] = [];
    let orderModels = factoryState.orderModels ?? [];

    orders = orderModels.map(order => Converter.orderModelToOrder(order));

    this._orderService._orders.next(orders);
    this._orderService.updateRunningOrders();
    return orders;
  }

  private subscribe() {
    this.factoryStateStreamService.updatedCell.subscribe(cell => this.updateCell(cell));
  }

  public selectCell(id: number | undefined) {
    const current = this._cellSelected.getValue();
    if (current && current.id === id || !id) {
      this._cellSelected.next(undefined);
      return;
    }

    const selectedCell = this._cells.find(c => c.id === id);
    this._cellSelected.next(selectedCell);
  }

  public async moveItem(item: VisualizableItemModel, update: CellLocationModel) {
    try {
      const location = await lastValueFrom(this.factoryMonitorService.moveCell({ body: update }));
      this.updateCell(<CellModel>{ id: item.id, location: location });
    } catch (error) {
      this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }
  }

  public getCell(cellId: number) : CellModel {
    const cell = this._cells.find(c => c.id === cellId)
    if (!cell) 
      throw Error(`Tried to process unknown cell with id ${cellId}`);
    return cell;
  }

  public getCells(factory: FactoryStateModel): CellModel[] {
    return this._cells.filter(c => c.factoryId === factory.id);
  }

  // Cell updates are retrieved as partial updates to the existing cell models, 
  // so we need to merge the incoming data with the existing cell data
  public updateCell(cell: CellModel) {
    const indexToUpdate = this._cells.findIndex(x => x.id === cell.id);
    let cellToUpdate = {... this._cells[indexToUpdate]};

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
      this._orderService.applyOrderColor(cellToUpdate);
    }
    if (cell.location) {
      cellToUpdate.location = cell.location;
    }

    this._cells[indexToUpdate] = cellToUpdate;
    this._cellUpdated.next(cellToUpdate);
  }
}

