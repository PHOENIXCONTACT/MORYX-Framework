/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { CellPropertySettings } from '@api/models/cell-property-settings';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-details-item',
    templateUrl: './details-item.html',
    styleUrls: ['./details-item.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatIconModule]
})
export class DetailsItem {
  readonly value = input.required<CellPropertySettings>();
  readonly name = input.required<string>();
}

