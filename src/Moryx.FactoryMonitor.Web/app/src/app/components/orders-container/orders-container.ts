/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import Order from 'src/app/models/order';

@Component({
    selector: 'app-orders-container',
    templateUrl: './orders-container.html',
    styleUrls: ['./orders-container.scss'],
    imports: [CommonModule]
})
export class OrdersContainer {
  TranslationConstants = TranslationConstants;
  private orderStoreService = inject(OrderStoreService);
  runningOrders = toSignal(this.orderStoreService.runningOrders$);

  toggleOrder(order: Order) {
    this.orderStoreService.toggleOrder(order);
  }
}

