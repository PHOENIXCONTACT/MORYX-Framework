/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, linkedSignal, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
  private activatedRoute = inject(ActivatedRoute);

  protected isEditMode = this.editProductsService.editing;
  protected currentProduct = this.editProductsService.currentProduct;
  protected currentRecipe = linkedSignal(this.editProductsService.currentRecipe);
  protected recipeDefinitions = this.cacheService.recipeDefinitions;
  protected TranslationConstants = TranslationConstants;

  private recipeId = signal(Number(this.activatedRoute.snapshot.paramMap.get('recipeId')));

  constructor() {
    // Track route param changes (component is reused when navigating between recipes)
    this.activatedRoute.paramMap.subscribe(params => {
      this.recipeId.set(Number(params.get('recipeId')));
    });

    // Resolve recipe when product loads or recipeId changes
    effect(() => {
      const product = this.editProductsService.currentProduct();
      const recipeId = this.recipeId();
      if (!product) {
        return;
      }
      untracked(() => {
        this.editProductsService.setRecipe(product.recipes?.find(r => r.id === recipeId));
      });
    });
  }

  protected updateRecipe(properties: Entry | undefined) {
    if (!properties) {
      return;
    }
    this.editProductsService.updateCurrentRecipe({... this.currentRecipe()!, properties});
  }
}
