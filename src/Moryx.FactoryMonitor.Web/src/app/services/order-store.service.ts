import { ApplicationRef, Injectable, NgZone, ɵgetUnknownElementStrictMode } from '@angular/core';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { OrderModel } from '../api/models/order-model';
import { FactoryStateStreamService } from './factory-state-stream.service';
import Order from '../models/order';
import { InternalOperationClassification } from '../api/models/internal-operation-classification';

@Injectable({
  providedIn: 'root',
})
export class OrderStoreService {
  public _orders = new BehaviorSubject<Order[]>([]);
  private _runningOrders = new BehaviorSubject<Order[]>([]);
  private _toggledOrder = new Subject<Order>();

  public orders$: Observable<Order[]>;
  public runningOrders$: Observable<Order[]>;
  public toggledOrder$: Observable<Order>;

  constructor(
    private factoryStateStreamService: FactoryStateStreamService) {
    this.factoryStateStreamService.updatedOrder.subscribe({
      next: order => {
        if (!order?.orderNumber) return;

        if (!this._orders.getValue().length) return;

        this.updateOrder(order);
      },
    });

    this.orders$ = this._orders.asObservable();
    this._runningOrders.next(this._orders.getValue().filter(o => o.classification == InternalOperationClassification.Running))
    this.runningOrders$ = this._runningOrders.asObservable();
    this.toggledOrder$ = this._toggledOrder.asObservable();
  }

  private getUpdateOrders(orders: Order[]): Order[] {
    // Unselect orders that existed but were unselected
    const currentlyUnselectedOrders = this._orders.getValue().filter(order => !order.isToggled);
    orders.forEach(
      no =>
        (no.isToggled = !!!currentlyUnselectedOrders.find(
          co =>
            no.orderNumber === co.orderNumber &&
            no.operationNumber === co.operationNumber
        ))
    );

    return orders;
  }

  public getOrder(orderNumber: string, operationNumber: string): Order | undefined {
    return this._orders
      .getValue()
      .find(o => o.operationNumber === operationNumber && o.orderNumber === orderNumber);
  }

  public toggleOrder(order: Order) {
    order.isToggled = !order.isToggled;
    this._toggledOrder.next(order);
    //this.appRef.tick();
  }

  //Groupes the orders to creates a Map<string,OrderModel[]>
  groupBy(list: OrderModel[], keyGetter: (orderType: OrderModel) => string) {
    const map = new Map();
    list.forEach(item => {
      const key = keyGetter(item);
      const collection = map.get(key);
      if (!collection) {
        map.set(key, [item]);
      } else {
        collection.push(item);
      }
    });
    return map;
  }

  // flatten the grouped order that has the format Map<string, OrderModel[]> to a simple OrderModel[]
  flattenOrders(groupedOrders: Map<string, OrderModel[]>) {
    let orders: OrderModel[] = [];
    groupedOrders.forEach((value: OrderModel[], key: string) =>
      orders.push(...orders, <OrderModel>{ order: key, operation: value[0]?.operation, color: value[0]?.color })
    );

    return orders;
  }

  public updateOrder(order: Order) {
    const orders = this._orders.getValue();
    const indexToUpdate = orders.findIndex(o => o.operationNumber === order.operationNumber && o.orderNumber === order.orderNumber);
    let orderToUpdate = orders[indexToUpdate];

    if(order.classification){
      orderToUpdate.classification = order.classification;
    }
    if(order.orderColor && order.orderColor != ''){
      orderToUpdate.orderColor = order.orderColor;
    }

    orders[indexToUpdate] = orderToUpdate;
    
    this._runningOrders.next(orders.filter(o => o.classification == InternalOperationClassification.Running))
    this._orders.next(orders)
  }

  public updateRunningOrders(){
    const orders = this._orders.getValue();
    this._runningOrders.next(orders.filter(o => o.classification == InternalOperationClassification.Running))
  }
}
