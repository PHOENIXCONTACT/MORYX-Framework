/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-orders-container',
    templateUrl: './orders-container.html',
    styleUrls: ['./orders-container.scss'],
    imports: [CommonModule],
    standalone: true
})
export class OrdersContainer {
  TranslationConstants = TranslationConstants;
  orderStoreService= inject(OrderStoreService);
  translate = inject(TranslateService);
}

