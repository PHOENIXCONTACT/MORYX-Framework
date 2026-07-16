/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { EditProductsService } from '@app/services/edit-products.service';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';

@Component({
  selector: 'app-product-parts-details',
  templateUrl: './product-parts-details.html',
  styleUrls: ['./product-parts-details.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    EmptyState,
    NavigableEntryEditor
  ]
})
export class ProductPartsDetailsComponent {
  private editProductsService = inject(EditProductsService);

  protected partConnector = linkedSignal(this.editProductsService.currentPartConnector);
  protected productPart = linkedSignal(this.editProductsService.currentPart);
  protected isEditMode = this.editProductsService.editing;
}

