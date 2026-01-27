/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import {
  ProductModel,
  RecipeDefinitionModel,
  RecipeModel
} from '../../../../api/models';
import { CacheProductsService } from '../../../../services/cache-products.service';
import { EditProductsService } from '../../../../services/edit-products.service';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework';
import {
  ProductRecipesDetailsHeaderComponent
} from './product-recipes-details-header/product-recipes-details-header.component';

@Component({
  selector: 'app-product-recipes-details',
  templateUrl: './product-recipes-details.component.html',
  styleUrls: ['./product-recipes-details.component.scss'],
  imports: [
    ProductRecipesDetailsHeaderComponent,
    NavigableEntryEditor,
    TranslateModule
  ],
  standalone: true
})
export class ProductRecipesDetailsComponent {
  currentProduct = signal<ProductModel | undefined>(undefined);
  currentRecipe = signal<RecipeModel | undefined>(undefined);
  recipeDefitions = signal<RecipeDefinitionModel[] | undefined>([]);
  TranslationConstants = TranslationConstants;

  constructor(
    public editService: EditProductsService,
    public route: ActivatedRoute,
    private router: Router,
    private cacheService: CacheProductsService,
    public translate: TranslateService
  ) {
    editService.currentProduct.subscribe((product) => {
      this.currentProduct.set(product);
      this.setCurrentRecipe();
      if (this.currentRecipe === undefined) {
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

    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.setCurrentRecipe();
      }
    });

    cacheService.recipeDefitions.subscribe((recipeDefitions) => {
      this.recipeDefitions.set(recipeDefitions);
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
