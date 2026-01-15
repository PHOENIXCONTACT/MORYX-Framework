/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, Input } from '@angular/core';
import { IconSize } from '../models/types';
import prettyMilliseconds from 'pretty-ms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-work-hours-icon',
  templateUrl: './work-hours-icon.component.html',
  styleUrl: './work-hours-icon.component.scss',
  standalone: true,
  imports : [
    CommonModule,
    MatIconModule,
    
  ]
})
export class WorkHoursIconComponent {
  orderHours = input.required<number>();
  operatorHours = input.required<number>();
  size = input.required<IconSize>();
}

