import { ApplicationRef, ChangeDetectorRef, Injectable, NgZone } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ActivityChangedModel } from '../api/models/activity-changed-model';
import { CellStateChangedModel } from '../api/models/cell-state-changed-model';
import { ResourceChangedModel } from '../api/models/resource-changed-model';
import { OrderModel } from '../api/models/order-model';
import { FactoryMonitorService } from '../api/services/factory-monitor.service';
import Cell from '../models/cell';
import Order from '../models/order';
import { Converter } from '../extensions/converter';
import { OrderChangedModel } from '../api/models/order-changed-model';

@Injectable({
  providedIn: 'root'
})
export class FactoryStateStreamService {
  updatedCell: BehaviorSubject<Cell | undefined> = new BehaviorSubject<Cell | undefined>(undefined);
  updatedOrder: BehaviorSubject<Order | undefined> = new BehaviorSubject<Order | undefined>(undefined);


  constructor(private factoryMonitorService: FactoryMonitorService, private zone: NgZone) {
    const eventSource = new EventSource(this.factoryMonitorService.rootUrl + FactoryMonitorService.FactoryStatesStreamPath);
    eventSource.onmessage = event => {
      const activityChangedModel = <ActivityChangedModel>JSON.parse(event.data);
      const resourceChangedModel = <ResourceChangedModel>JSON.parse(event.data);
      const cellStateChangedModel = <CellStateChangedModel>JSON.parse(event.data);
      const orderModel = <OrderModel>JSON.parse(event.data);
      const orderChangedModel = <OrderChangedModel>JSON.parse(event.data);
      //const orderReferenceModel = <OrderReferenceModel>JSON.parse(event.data);

      if(activityChangedModel?.resourceId){
        const cell = Converter.activityChangedModelToCell(activityChangedModel);
        this.zone.run(() => this.updatedCell.next(cell));
      }
      else if(resourceChangedModel?.cellName){
        const cell = Converter.resourceChangedModelToCell(resourceChangedModel);
        this.zone.run(() => this.updatedCell.next(cell));
      }
      else if(cellStateChangedModel?.id &&cellStateChangedModel?.state){
        const cell = Converter.cellStateChangedModelToCell(cellStateChangedModel);
        this.zone.run(() => this.updatedCell.next(cell));
      }
      else if(orderModel?.color){
        const order = Converter.orderModelToOrder(orderModel);
        this.zone.run(() => this.updatedOrder.next(order));
      }
      else if(orderChangedModel?.state){
        const order = Converter.orderChangedModelToOrder(orderChangedModel);
        this.zone.run(() => this.updatedOrder.next(order));
      }
    };
  }

  
}
