/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input } from '@angular/core';
import { CellPropertySettings } from 'src/app/api/models/cell-property-settings';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-details-item',
    templateUrl: './details-item.html',
    styleUrls: ['./details-item.scss'],
    imports: [CommonModule],
    standalone: true
})
export class DetailsItem {
  value = input.required<CellPropertySettings>();
  name = input.required<string>();
}

