/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { ActivityChangedModel } from '@api/models/activity-changed-model';
import { CellStateChangedModel } from '@api/models/cell-state-changed-model';
import { ResourceChangedModel } from '@api/models/resource-changed-model';
import { OrderModel } from '@api/models/order-model';
import { FactoryMonitorService } from '@api/services/factory-monitor.service';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { Converter } from '../extensions/converter';
import { OrderChangedModel } from '@api/models/order-changed-model';

@Injectable({
  providedIn: 'root'
})
export class FactoryStateStreamService {
  private readonly factoryMonitorService = inject(FactoryMonitorService);
  private readonly Order_Event_Type_Key = "order";
  private readonly Order_Changed_Event_Type_Key = "orderChanged";
  private readonly Cell_State_Event_Type_Key = "cellStateChangedModel";
  private readonly Activity_Event_Type_Key = "activityChangedModel";
  private readonly Recource_Event_Type_Key = "resourceChangedModel";

  private eventSource?: EventSource;

  // ToDo: Only make observable public
  updatedCell: ReplaySubject<CellModel> = new ReplaySubject<CellModel>();
  updatedOrder: ReplaySubject<Order> = new ReplaySubject<Order>();

  connect() {
    this.eventSource = new EventSource(this.factoryMonitorService.rootUrl + FactoryMonitorService.FactoryStatesStreamPath);

    this.eventSource.addEventListener(this.Order_Event_Type_Key, (event: MessageEvent<string>) => {
      this.transformOrderEvent(event);
    });

    this.eventSource.addEventListener(this.Order_Changed_Event_Type_Key, (event: MessageEvent<string>) => {
      this.transformOrderChangedEvent(event);
    });

    this.eventSource.addEventListener(this.Cell_State_Event_Type_Key, (event: MessageEvent<string>) => {
      this.transformCellStateChangedEvent(event);
    });

    this.eventSource.addEventListener(this.Activity_Event_Type_Key, (event: MessageEvent<string>) => {
      this.transformActivityChangedEvent(event);
    });

    this.eventSource.addEventListener(this.Recource_Event_Type_Key, (event: MessageEvent<string>) => {
      this.transformResourceEvent(event);
    });
  }

  private transformResourceEvent(event: MessageEvent<string>) {
    const resourceChangedModel = <ResourceChangedModel>JSON.parse(event.data);
    const cell = Converter.resourceChangedModelToCell(resourceChangedModel);
    this.updatedCell.next(cell);
  }

  private transformActivityChangedEvent(event: MessageEvent<string>) {
    const activityChangedModel = <ActivityChangedModel>JSON.parse(event.data);
    const cell = Converter.activityChangedModelToCell(activityChangedModel);
    this.updatedCell.next(cell);
  }

  private transformCellStateChangedEvent(event: MessageEvent<string>) {
    const cellStateChangedModel = <CellStateChangedModel>JSON.parse(event.data);
    const cell = Converter.cellStateChangedModelToCell(cellStateChangedModel);
    this.updatedCell.next(cell);
  }

  private transformOrderChangedEvent(event: MessageEvent<string>) {
    const orderChangedModel = <OrderChangedModel>JSON.parse(event.data);
    const order = Converter.orderChangedModelToOrder(orderChangedModel);
    this.updatedOrder.next(order);
  }

  private transformOrderEvent(event: MessageEvent<string>) {
    const orderModel = <OrderModel>JSON.parse(event.data);
    const order = Converter.orderModelToOrder(orderModel);
    this.updatedOrder.next(order);
  }

  disconnect() {
    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
    }
  }
}
