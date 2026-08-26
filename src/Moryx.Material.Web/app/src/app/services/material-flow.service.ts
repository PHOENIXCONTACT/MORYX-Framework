import { inject, Injectable } from '@angular/core';
import { MaterialContainer } from '../models/material-container';
import { BehaviorSubject } from 'rxjs';
import { MaterialManagementService } from '../api/services';
import { MaterialContainerModel, OrderReferenceModel, ResourceModel } from '../api/models';

@Injectable({
  providedIn: 'root',
})
export class MaterialFlowService {
  private filter = new BehaviorSubject<string[]>([]);
  $filter = this.filter.asObservable();
  private linkedOrders = new BehaviorSubject<OrderReferenceModel[]>([]);
  $linkedOrders = this.linkedOrders.asObservable();

  executeFilter(filter: string[]) {
    this.filter.next(filter);
  }

  updateLinkedOrders(newOrders: OrderReferenceModel[]) 
  {
    this.linkedOrders.next(newOrders);
  }
}