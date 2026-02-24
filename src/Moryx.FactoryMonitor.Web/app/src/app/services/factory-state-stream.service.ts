/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ActivityChangedModel } from '../api/models/activity-changed-model';
import { CellStateChangedModel } from '../api/models/cell-state-changed-model';
import { ResourceChangedModel } from '../api/models/resource-changed-model';
import { OrderModel } from '../api/models/order-model';
import { FactoryMonitorService } from '../api/services/factory-monitor.service';
import CellModel from '../models/cellModel';
import Order from '../models/order';
import { Converter } from '../extensions/converter';
import { OrderChangedModel } from '../api/models/order-changed-model';

@Injectable({
  providedIn: 'root'
})
export class FactoryStateStreamService {
  private factoryMonitorService = inject(FactoryMonitorService);
  updatedCell: BehaviorSubject<CellModel | undefined> = new BehaviorSubject<CellModel | undefined>(undefined);
  updatedOrder: BehaviorSubject<Order | undefined> = new BehaviorSubject<Order | undefined>(undefined);

  constructor() {
    const eventSource = new EventSource(this.factoryMonitorService.rootUrl + FactoryMonitorService.FactoryStatesStreamPath);
    eventSource.onmessage = event => {
      const activityChangedModel = <ActivityChangedModel>JSON.parse(event.data);
      const resourceChangedModel = <ResourceChangedModel>JSON.parse(event.data);
      const cellStateChangedModel = <CellStateChangedModel>JSON.parse(event.data);
      const orderModel = <OrderModel>JSON.parse(event.data);
      const orderChangedModel = <OrderChangedModel>JSON.parse(event.data);

      if(activityChangedModel?.resourceId){
        const cell = Converter.activityChangedModelToCell(activityChangedModel);
        this.updatedCell.next(cell);
      }
      else if(resourceChangedModel?.cellName){
        const cell = Converter.resourceChangedModelToCell(resourceChangedModel);
        this.updatedCell.next(cell);
      }
      else if(cellStateChangedModel?.id &&cellStateChangedModel?.state){
        const cell = Converter.cellStateChangedModelToCell(cellStateChangedModel);
        this.updatedCell.next(cell);
      }
      else if(orderModel?.color){
        const order = Converter.orderModelToOrder(orderModel);
        this.updatedOrder.next(order);
      }
      else if(orderChangedModel?.state){
        const order = Converter.orderChangedModelToOrder(orderChangedModel);
        this.updatedOrder.next(order);
      }
    };
  }


}

