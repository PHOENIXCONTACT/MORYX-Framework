/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { CacheProductsService } from '@app/services/cache-products.service';
import { EditProductsService } from '@app/services/edit-products.service';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { ProductRecipesDetailsHeader } from './product-recipes-details-header/product-recipes-details-header';

@Component({
  selector: 'app-product-recipes-details',
  templateUrl: './product-recipes-details.html',
  styleUrls: ['./product-recipes-details.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    ProductRecipesDetailsHeader,
    NavigableEntryEditor,
    TranslatePipe
  ]
})
export class ProductRecipesDetails {
  private editProductsService = inject(EditProductsService);
  private cacheService = inject(CacheProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  currentProduct = toSignal(this.editProductsService.currentProduct$);
  currentRecipe = linkedSignal(this.editProductsService.currentRecipe);
  recipeDefinitions = toSignal(this.cacheService.recipeDefinitions, { initialValue: [] });
  TranslationConstants = TranslationConstants;

  updateRecipe(properties: Entry | undefined) {
    if (!properties) return;
    this.editProductsService.updateCurrentRecipe({... this.currentRecipe()!, properties});
  }
}
