/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { VariantDescriptor } from '@api/models';

@Component({
  selector: 'app-variant-details-header',
  templateUrl: './variant-details-header.html',
  styleUrls: ['./variant-details-header.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule]
})
export class VariantDetailsHeader {
  readonly selectedVariant = input.required<VariantDescriptor>();
}
