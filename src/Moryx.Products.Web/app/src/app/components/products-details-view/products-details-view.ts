/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from '@angular/core';
import { ActivatedRoute, NavigationCancel, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { SessionService } from 'src/app/services/session.service';
import { ProductModel } from '../../api/models';
import { EditProductsService } from '../../services/edit-products.service';
import { ProductsDetailsHeaderComponent } from './products-details-header/products-details-header';

import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
    selector: 'app-products-details-view',
    templateUrl: './products-details-view.html',
    styleUrls: ['./products-details-view.scss'],
    standalone: true,
    imports: [
    ProductsDetailsHeaderComponent,
    MatDividerModule,
    MatTabsModule,
    TranslateModule,
    RouterOutlet
]
})
export class ProductsDetailsViewComponent {
  currentProduct = signal<ProductModel | undefined>(undefined);
  lastProductId = signal<number | undefined>(undefined);
  activeLink = signal<Tabs>(Tabs.Unknown);

  Tabs = Tabs;
  TranslationConstants = TranslationConstants;
  regexParts: RegExp = /(details\/\d*\/parts)/;
  regexRecipes: RegExp = /(details\/\d*\/recipes)/;
  regexReferences: RegExp = /(details\/\d*\/references)/;
  regexProperties: RegExp = /(details\/\d*\/properties)/;

  constructor(
    private router: Router,
    private sessionService: SessionService,
    public editService: EditProductsService,
    public route: ActivatedRoute,
    public translate: TranslateService
  ) {
    this.route.data.subscribe(data => {
      const product = data['product'];

      if (this.lastProductId() === product?.id) return;
      if (product) this.currentProduct.set(product);
      
      const wipProduct = this.sessionService.getWipProduct();
      
      if (
        this.lastProductId() !== undefined &&
        product?.properties &&
        !wipProduct
      ) {
        const newUrl = `details/${product?.id}/properties`;
        this.router.navigate([newUrl]);
      }
      
      this.lastProductId.set(product?.id);
      
      if (wipProduct) {
        this.editService.edit = true;
        this.sessionService.removeWipProduct();
      }
    });

    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd || val instanceof NavigationCancel) {
        let url = this.router.url;
        if (this.regexProperties.test(url)) {
          this.activeLink.update(_=> Tabs.Properties);
        } else if (this.regexParts.test(url)) {
          this.activeLink.update(_=> Tabs.Parts);
        } else if (this.regexRecipes.test(url)) {
          this.activeLink.update(_=> Tabs.Recipes);
        } else if (this.regexReferences.test(url)) {
          this.activeLink.update(_=> Tabs.References);
        }
      }
    });
  }

  routeTo(target: number) {
    const url = this.router.url;
    const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
    const regexParts: RegExp = /(details\/\d*\/parts)/;
    if (regexSpecificRecipe.test(url) || regexParts.test(url)) {
      this.router.navigate(['../../'], { relativeTo: this.route }).then(() => {
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

