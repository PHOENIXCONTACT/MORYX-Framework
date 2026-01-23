/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, Input, signal } from '@angular/core';
import { CellPropertySettings } from 'src/app/api/models/cell-property-settings';
import { CommonModule, NgStyle } from '@angular/common';

@Component({
    selector: 'app-details-item',
    templateUrl: './details-item.component.html',
    styleUrls: ['./details-item.component.scss'],
    imports: [CommonModule],
    standalone: true
})
export class DetailsItemComponent {
  value = input.required<CellPropertySettings>(); 
  name = input.required<string>();
}

