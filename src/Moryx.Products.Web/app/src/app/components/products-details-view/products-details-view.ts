/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, NavigationCancel, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { EditProductsService } from '@app/services/edit-products.service';
import { SessionService } from '@app/services/session.service';
import { ProductManagementService } from '@app/api/services';
import { ProductsDetailsHeader } from './products-details-header/products-details-header';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';

import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-products-details-view',
  templateUrl: './products-details-view.html',
  styleUrls: ['./products-details-view.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    ProductsDetailsHeader,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    TranslatePipe,
    RouterOutlet
  ]
})
export class ProductsDetailsView {
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private editProductsService = inject(EditProductsService);
  private productManagementService = inject(ProductManagementService);
  private sessionService = inject(SessionService);
  private snackbarService = inject(SnackbarService);

  protected loading = signal(true);
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
    this.activatedRoute.paramMap.subscribe(params => {
      this.loadProduct(Number(params.get('id')));
    });

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

  private async loadProduct(id: number) {
    this.loading.set(true);

    // Restore work-in-progress product from session storage
    const workInProgress = this.sessionService.popWipProduct();
    if (workInProgress?.product.id === id) {
      this.editProductsService.setProductFromStorage(workInProgress);
      this.loading.set(false);
      return;
    }

    try {
      const product = await this.productManagementService.getTypeById({ id });
      this.editProductsService.setProduct(product);
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
      this.editProductsService.resetProduct();
      this.router.navigate(['']);
    } finally {
      this.loading.set(false);
    }
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
}

enum Tabs {
  Unknown,
  Properties,
  Parts,
  Recipes,
  References,
}

