/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { TranslationConstants } from '@app/translation-constants';
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
    NavigableEntryEditor
  ]
})
export class ProductRecipesDetails {
  private editProductsService = inject(EditProductsService);
  private cacheService = inject(CacheProductsService);

  protected isEditMode = this.editProductsService.editing;
  protected currentProduct = this.editProductsService.currentProduct;
  protected currentRecipe = linkedSignal(this.editProductsService.currentRecipe);
  protected recipeDefinitions = this.cacheService.recipeDefinitions;
  protected TranslationConstants = TranslationConstants;

  protected updateRecipe(properties: Entry | undefined) {
    if (!properties) {
      return;
    }
    this.editProductsService.updateCurrentRecipe({... this.currentRecipe()!, properties});
  }
}
