/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Input } from '@angular/core';
import { OrderModel } from '../models/order-model';
import prettyMilliseconds from 'pretty-ms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-order-item',
  templateUrl: './order-item.component.html',
  styleUrl: './order-item.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule
  ]
})
export class OrderItemComponent {
  prettyMilliseconds = prettyMilliseconds;
  hourToMillisecond = 3600000;
  @Input() order!: OrderModel;
}

