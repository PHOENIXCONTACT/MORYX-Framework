/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, distinctUntilChanged, map, shareReplay, Subject } from 'rxjs';
import { FactoryStateModel } from '../api/models';
import { InternalOperationClassification } from '../api/models/internal-operation-classification';
import { Converter } from '../extensions/converter';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { FactoryStateStreamService } from './factory-state-stream.service';

@Injectable({
  providedIn: 'root',
})
export class OrderStoreService {
  private factoryStateStreamService = inject(FactoryStateStreamService);

  private readonly _orders = new BehaviorSubject<Order[]>([]);
  private readonly _toggledOrder = new Subject<Order>();

  public readonly toggledOrder$ = this._toggledOrder.asObservable();
  public readonly runningOrders$ = this._orders.pipe(
    map(orders => orders.filter(o => o.classification === InternalOperationClassification.Running)),
    // Prevent other order state changes from triggering updates in the UI 
    // ToDo: Reanble when OrderManagement facade fires order-started event before order-changed-to-running event
    // distinctUntilChanged((previousOrders, newOrders) => this.areSameSet(previousOrders, newOrders)),
    shareReplay(1)
  );
  // private areSameSet(previousOrders: Order[], newOrders: Order[]): boolean {
  //   return previousOrders.length === newOrders.length && previousOrders.every((x, i) => x.orderNumber === newOrders[i].orderNumber && x.operationNumber === newOrders[i].operationNumber);
  // }

  constructor() {
    this.factoryStateStreamService.updatedOrder.subscribe(order => this.updateOrder(order));
  }

  public initialize(factoryState: FactoryStateModel) {
    const orderModels = factoryState.orderModels ?? [];

    const orders = orderModels.map(order => Converter.orderModelToOrder(order));

    this._orders.next(orders);
  }

  // We update orders partially to retain the toggled state through order updates from the backend
  public updateOrder(order: Order) {
    if (!order?.orderNumber || !order.operationNumber) {
      return;
    }

    const orders = this._orders.getValue();
    let indexToUpdate = orders.findIndex(o => o.operationNumber === order.operationNumber && o.orderNumber === order.orderNumber);
    if(indexToUpdate === -1) {
      orders.push(order);
    } else {
      
      let orderToUpdate = orders[indexToUpdate];

      if (order.classification) {
        orderToUpdate.classification = order.classification;
      }
      if (order.orderColor && order.orderColor != '') {
        orderToUpdate.orderColor = order.orderColor;
      }

      orders[indexToUpdate] = orderToUpdate;
    }

    this._orders.next(orders)
  }

  public getOrder(cell: CellModel): Order | undefined {
    if (!cell?.orderNumber || !cell.operationNumber) {
      return undefined;
    }

    return this._orders.getValue()
      .find(o => o.operationNumber === cell.operationNumber && o.orderNumber === cell.orderNumber);
  }

  public toggleOrder(order: Order) {
    order.isToggled = !order.isToggled;
    this._toggledOrder.next(order);
  }

  public applyOrderColor(cell: CellModel): CellModel {
    const color = this._orders.getValue().find(o => o.operationNumber === cell.operationNumber && o.orderNumber === cell.orderNumber)?.orderColor;
    cell.orderColor = color ?? '';
    return cell;
  }
}
