/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { TranslationConstants } from '@app/translation-constants';
import { OrderStoreService } from '@app/services/order-store.service';
import { CommonModule } from '@angular/common';
import Order from '@app/models/order';

@Component({
    selector: 'app-orders-container',
    templateUrl: './orders-container.html',
    styleUrls: ['./orders-container.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [CommonModule]
})
export class OrdersContainer {
  protected TranslationConstants = TranslationConstants;
  private orderStoreService = inject(OrderStoreService);
  protected runningOrders = this.orderStoreService.runningOrders;

  protected toggleOrder(order: Order) {
    this.orderStoreService.toggleOrder(order);
  }
}

