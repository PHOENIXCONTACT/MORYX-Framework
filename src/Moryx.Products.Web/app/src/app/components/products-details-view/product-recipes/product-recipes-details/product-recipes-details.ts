/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { CacheProductsService } from '../../../../services/cache-products.service';
import { EditProductsService } from '../../../../services/edit-products.service';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { ProductRecipesDetailsHeader } from './product-recipes-details-header/product-recipes-details-header';
import { RecipeModel } from 'src/app/api/models';

@Component({
  selector: 'app-product-recipes-details',
  templateUrl: './product-recipes-details.html',
  styleUrls: ['./product-recipes-details.scss'],
  imports: [
    ProductRecipesDetailsHeader,
    NavigableEntryEditor,
    TranslateModule
  ]
})
export class ProductRecipesDetails {
  private editProductsService = inject(EditProductsService);
  private cacheService = inject(CacheProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  currentProduct = toSignal(this.editProductsService.currentProduct$, { initialValue: undefined });
  currentRecipe = toSignal(this.editProductsService.currentRecipe$, { initialValue: undefined });
  recipeDefinitions = toSignal(this.cacheService.recipeDefinitions, { initialValue: [] });
  TranslationConstants = TranslationConstants;

  updateRecipe(properties: Entry | undefined) {
    if (!properties) return;
    this.editProductsService.updateCurrentRecipe({... this.currentRecipe()!, properties});
  }
}
