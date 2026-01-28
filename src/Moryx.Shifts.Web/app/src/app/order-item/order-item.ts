/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Input } from '@angular/core';
import { OrderModel } from '../models/order-model';
import prettyMilliseconds from 'pretty-ms';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-order-item',
  templateUrl: './order-item.html',
  styleUrl: './order-item.scss',
  standalone: true,
  imports: [
    MatIconModule
]
})
export class OrderItem {
  prettyMilliseconds = prettyMilliseconds;
  hourToMillisecond = 3600000;
  @Input() order!: OrderModel;
}

