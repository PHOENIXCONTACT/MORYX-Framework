/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, effect, inject, Injectable, signal, untracked } from '@angular/core';
import { FactoryStateModel } from '@api/models';
import { InternalOperationClassification } from '@api/models/internal-operation-classification';
import { Converter } from '../extensions/converter';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { FactoryStateStreamService } from './factory-state-stream.service';

@Injectable({
  providedIn: 'root',
})
export class OrderStoreService {
  private readonly factoryStateStreamService = inject(FactoryStateStreamService);

  private readonly orders = signal<Order[]>([]);
  private readonly _toggledOrder = signal<Order | undefined>(undefined);
  readonly toggledOrder = this._toggledOrder.asReadonly();

  // TODO: Add custom `equal` fn when OrderManagement facade fires order-started event before order-changed-to-running event
  readonly runningOrders = computed(() =>
    this.orders().filter(o => o.classification === InternalOperationClassification.Running)
  );

  constructor() {
    effect(() => {
      const order = this.factoryStateStreamService.updatedOrder();
      if (order) {
        untracked(() => {
          this.updateOrder(order);
        });
      }
    });
  }

  public initialize(factoryState: FactoryStateModel) {
    const orderModels = factoryState.orderModels ?? [];
    const orders = orderModels.map(order => Converter.orderModelToOrder(order));
    this.orders.set(orders);
  }

  // We update orders partially to retain the toggled state through order updates from the backend
  public updateOrder(order: Order) {
    if (!order?.orderNumber || !order.operationNumber) {
      return;
    }

    this.orders.update(orders => {
      const copy = [...orders];
      const indexToUpdate = copy.findIndex(o => o.operationNumber === order.operationNumber && o.orderNumber === order.orderNumber);
      if (indexToUpdate === -1) {
        copy.push(order);
      } else {
        const orderToUpdate = {...copy[indexToUpdate]};

        if (order.classification) {
          orderToUpdate.classification = order.classification;
        }
        if (order.orderColor && order.orderColor != '') {
          orderToUpdate.orderColor = order.orderColor;
        }

        copy[indexToUpdate] = orderToUpdate;
      }
      return copy;
    });
  }

  public getOrder(cell: CellModel): Order | undefined {
    if (!cell?.orderNumber || !cell.operationNumber) {
      return undefined;
    }

    return this.orders()
      .find(o => o.operationNumber === cell.operationNumber && o.orderNumber === cell.orderNumber);
  }

  public toggleOrder(order: Order) {
    order.isToggled = !order.isToggled;
    this._toggledOrder.set(order);
  }

  public applyOrderColor(cell: CellModel): CellModel {
    const color = this.orders().find(o => o.operationNumber === cell.operationNumber && o.orderNumber === cell.orderNumber)?.orderColor;
    cell.orderColor = color ?? '';
    return cell;
  }
}
