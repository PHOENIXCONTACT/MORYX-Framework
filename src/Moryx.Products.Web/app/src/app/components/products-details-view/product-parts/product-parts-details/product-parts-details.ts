/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
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
    NavigableEntryEditor,
    TranslatePipe
  ]
})
export class ProductPartsDetailsComponent {
  private editProductsService = inject(EditProductsService);

  partConnector = linkedSignal(this.editProductsService.currentPartConnector);
  productPart = linkedSignal(this.editProductsService.currentPart);
  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });

  TranslationConstants = TranslationConstants;

  createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editProductsService.createProductIdentity(identifier, revision);
  }
}

