/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ProductModel, RecipeDefinitionModel, RecipeModel } from '../../../../api/models';
import { CacheProductsService } from '../../../../services/cache-products.service';
import { EditProductsService } from '../../../../services/edit-products.service';
import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { ProductRecipesDetailsHeader } from './product-recipes-details-header/product-recipes-details-header';

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
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cacheService = inject(CacheProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  currentProduct = signal<ProductModel | undefined>(undefined);
  currentRecipe = signal<RecipeModel | undefined>(undefined);
  recipeDefinitions = signal<RecipeDefinitionModel[] | undefined>([]);
  TranslationConstants = TranslationConstants;

  constructor() {
    // ToDo: Add recipe resolver and map recipe directly to signal
    this.editProductsService.currentProduct$.subscribe((product) => {
      this.currentProduct.set(product);
      this.setCurrentRecipe();
      if (this.currentRecipe() === undefined) {
        let url = this.router.url;
        // If the current route is a child child route, move to the parent route first
        // in order to have no "Cannot match any routes. URL Segment:" error
        const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
        if (regexSpecificRecipe.test(url)) {
          this.router
            .navigate(['../../'], {relativeTo: this.route})
            .then(() => {
              this.routeToDefault(url);
            });
        } else {
          this.routeToDefault(url);
        }
      }
    });

    this.router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.setCurrentRecipe();
      }
    });

    this.cacheService.recipeDefinitions.subscribe((recipeDefitions) => {
      this.recipeDefinitions.set(recipeDefitions);
    });
  }

  ngOnInit(): void {
  }

  private routeToDefault(url: string) {
    const index = url.lastIndexOf('recipes');
    let newUrl = url.substring(0, index);
    newUrl += 'recipes/';
    this.router.navigate([newUrl]);
  }

  private setCurrentRecipe(): void {
    const id = Number(this.route.snapshot.paramMap.get('recipeId'));
    this.currentRecipe.set(this.currentProduct()?.recipes?.find((r) => r.id === id));
  }
}
