/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, PartModel } from '../../../../api/models';
import { EditProductsService } from '../../../../services/edit-products.service';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-product-parts-details',
  templateUrl: './product-parts-details.html',
  styleUrls: ['./product-parts-details.scss'],
  imports: [
    NavigableEntryEditor,
    TranslateModule
  ]
})
export class ProductPartsDetailsComponent {
  private editProductsService = inject(EditProductsService);

  // ToDo: Replace with service injection
  partConnector = input.required<PartConnector>();
  productPart = input.required<PartModel>();

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });

  TranslationConstants = TranslationConstants;

  createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editProductsService.createProductIdentity(identifier, revision);
  }
}

