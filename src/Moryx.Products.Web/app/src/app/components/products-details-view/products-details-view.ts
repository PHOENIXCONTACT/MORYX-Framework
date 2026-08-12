/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, NavigationCancel, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { EditProductsService } from '@app/services/edit-products.service';
import { ProductsDetailsHeader } from './products-details-header/products-details-header';

import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';
import { ProductModel } from '@api/models';

@Component({
  selector: 'app-products-details-view',
  templateUrl: './products-details-view.html',
  styleUrls: ['./products-details-view.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    ProductsDetailsHeader,
    MatDividerModule,
    MatTabsModule,
    TranslatePipe,
    RouterOutlet
  ]
})
export class ProductsDetailsView {
  private router = inject(Router);
  private editProductsService = inject(EditProductsService);
  private activatedRoute = inject(ActivatedRoute);

  protected isEditMode = this.editProductsService.editing;
  protected currentProduct = this.editProductsService.currentProduct;
  protected activeLink = signal<Tabs>(Tabs.Unknown);

  protected Tabs = Tabs;
  protected TranslationConstants = TranslationConstants;
  private regexParts: RegExp = /(details\/\d*\/parts)/;
  private regexRecipes: RegExp = /(details\/\d*\/recipes)/;
  private regexReferences: RegExp = /(details\/\d*\/references)/;
  private regexProperties: RegExp = /(details\/\d*\/properties)/;

  constructor() {
    this.router.events.subscribe((val) => {
      if (val instanceof NavigationEnd || val instanceof NavigationCancel) {
        const url = this.router.url;
        if (this.regexProperties.test(url)) {
          this.activeLink.set(Tabs.Properties);
        } else if (this.regexParts.test(url)) {
          this.activeLink.set(Tabs.Parts);
        } else if (this.regexRecipes.test(url)) {
          this.activeLink.set(Tabs.Recipes);
        } else if (this.regexReferences.test(url)) {
          this.activeLink.set(Tabs.References);
        }
      }
    });
  }

  protected routeTo(target: number) {
    const url = this.router.url;
    const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
    const regexParts: RegExp = /(details\/\d*\/parts)/;
    // ToDo: Simplify, no need for 2 navigations
    if (regexSpecificRecipe.test(url) || regexParts.test(url)) {
      this.router.navigate(['../../'], { relativeTo: this.activatedRoute }).then(() => {
        this.routeToTab(target);
      });
    } else {
      this.routeToTab(target);
    }
  }

  private routeToTab(target: Tabs) {
    let url = '';
    switch (target) {
      case Tabs.Properties:
        url = '/details/' + this.currentProduct()?.id + '/properties';
        break;
      case Tabs.Parts:
        url = '/details/' + this.currentProduct()?.id + '/parts/base/0';
        break;
      case Tabs.Recipes:
        url = '/details/' + this.currentProduct()?.id + '/recipes';
        break;
      case Tabs.References:
        url = '/details/' + this.currentProduct()?.id + '/references';
        break;
    }

    this.router.navigate([url]);
  }

  protected onCurrentProductChangeFromHeader(product: ProductModel | undefined) {
    if (this.isEditMode() && product) {
      this.editProductsService.updateCurrentProduct(product);
    }
  }
}

enum Tabs {
  Unknown,
  Properties,
  Parts,
  Recipes,
  References,
}

