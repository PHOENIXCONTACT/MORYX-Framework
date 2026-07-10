/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { IconSize } from '../models/types';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-work-hours-icon',
  templateUrl: './work-hours-icon.html',
  styleUrl: './work-hours-icon.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports : [
    MatIconModule,
  ]
})
export class WorkHoursIcon {
  readonly orderHours = input.required<number>();
  readonly operatorHours = input.required<number>();
  readonly size = input.required<IconSize>();
}

