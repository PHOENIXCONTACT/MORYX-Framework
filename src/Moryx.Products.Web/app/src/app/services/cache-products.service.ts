/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { firstValueFrom } from 'rxjs';
import {
  ProductDefinitionModel,
  ProductImporter,
  WorkplanModel,
  ProductModel,
  RecipeDefinitionModel,
  RevisionFilter,
  Selector,
  ProductQuery,
} from '@api/models';
import { ProductManagementService } from '@api/services/product-management.service';
import { FilterOptions } from '../models/FilterOptions';
import { WorkplanService } from '@api/services/workplan.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from '../translation-constants';
import { HttpErrorResponse } from '@angular/common/http';
import { Import$Params } from '@api/functions';

@Injectable({
  providedIn: 'root',
})
export class CacheProductsService {
  private service = inject(ProductManagementService);
  private workplanService = inject(WorkplanService);
  private router = inject(Router);
  private snackbarService = inject(SnackbarService);
  private translateService = inject(TranslateService);

  private readonly _definitions = signal<ProductDefinitionModel[] | undefined>(undefined);
  readonly definitions = this._definitions.asReadonly();
  private readonly _productsShownInTheTree = signal<ProductModel[] | undefined>(undefined);
  readonly productsShownInTheTree = this._productsShownInTheTree.asReadonly();
  private readonly _importers = signal<ProductImporter[] | undefined>(undefined);
  readonly importers = this._importers.asReadonly();
  private readonly _recipeDefinitions = signal<RecipeDefinitionModel[] | undefined>(undefined);
  readonly recipeDefinitions = this._recipeDefinitions.asReadonly();
  private readonly _workplans = signal<WorkplanModel[] | undefined>(undefined);
  readonly workplans = this._workplans.asReadonly();

  protected TranslationConstants = TranslationConstants;

  public filterOptions: FilterOptions = {
    name: '',
    identifier: '',
    revision: RevisionFilter.Latest,
    selector: Selector.Direct,
  } as FilterOptions;

  loadConfiguration() {
    this.service.getProductCustomization()
      .then((configuration) => {
        if (configuration.importers !== null) {
          this._importers.set(configuration.importers);
        }
        if (configuration.productTypes !== null) {
          this._definitions.set(configuration.productTypes);
        }
        if (configuration.recipeTypes !== null) {
          this._recipeDefinitions.set(configuration.recipeTypes);
        }
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
      });

    this.workplanService.getAllWorkplans()
      .then((workplans) => {
        this._workplans.set(workplans);
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
      });
  }

  // ToDo Make async
  loadProductsForTree() {
    let body = <ProductQuery>{};
    if (this.filterOptions.name && this.filterOptions.identifier) {
      body = {
        includeDeleted: false,
        name: this.filterOptions.name,
        identifier: this.filterOptions.identifier,
        revisionFilter:
          RevisionFilter[
            this.filterOptions.revision as keyof typeof RevisionFilter
            ],
        selector:
          Selector[this.filterOptions.selector as keyof typeof Selector],
      };
    } else if (this.filterOptions.name) {
      body = {
        includeDeleted: false,
        name: this.filterOptions.name,
        revisionFilter:
          RevisionFilter[
            this.filterOptions.revision as keyof typeof RevisionFilter
            ],
        selector:
          Selector[this.filterOptions.selector as keyof typeof Selector],
      };
    } else if (this.filterOptions.identifier) {
      body = {
        includeDeleted: false,
        identifier: this.filterOptions.identifier,
        revisionFilter:
          RevisionFilter[
            this.filterOptions.revision as keyof typeof RevisionFilter
            ],
        selector:
          Selector[this.filterOptions.selector as keyof typeof Selector],
      };
    } else {
      body = {
        includeDeleted: false,
        revisionFilter:
          RevisionFilter[
            this.filterOptions.revision as keyof typeof RevisionFilter
            ],
        selector:
          Selector[this.filterOptions.selector as keyof typeof Selector],
      };
    }

    this.service.getTypes({body: body})
      .then((products) => {
        if (products !== null) {
          this._productsShownInTheTree.set(products);
        }
      })
      .catch(() => this.showErrorSnackbar());
  }

  private async showErrorSnackbar() {
    const translations = await firstValueFrom(this.translateService
      .get([
        TranslationConstants.APP.FAILED_LOADING,
        TranslationConstants.DISMISS,
      ]));
    await this.snackbarService.showError(
      translations[TranslationConstants.APP.FAILED_LOADING]
    );
  }

  resetFilter() {
    this.filterOptions.identifier = '';
    this.filterOptions.name = '';
    this.filterOptions.revision = RevisionFilter.Latest;
    this.filterOptions.selector = Selector.Direct;
    this.loadProductsForTree();
  }

  async deleteProduct(product: ProductModel) {
    if (!product.id) {
      return;
    }

    let success: boolean = false;
    await this.service
      .deleteType({id: product.id})
      .then((res) => (success = res))
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    if (!success) {
      return;
    }

    let newProductsForTree = this.productsShownInTheTree() ?? [];
    //Check if an older revision exists and, if yes, show that one
    newProductsForTree = newProductsForTree.filter((r) => r.id != product.id);
    const body = {
      identifier: product.identifier,
      revisionFilter: RevisionFilter.Latest,
    };

    await this.service
      .getTypes({body: body})
      .then((results) => {
        if (results && results[0]) {
          const existingRevision = newProductsForTree.find(p => p.id === results[0].id);
          if (!existingRevision) {
            const otherRevision = results[0];
            newProductsForTree.push(otherRevision);
          }
        }
        this._productsShownInTheTree.set(newProductsForTree);
      })
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );

    //check if current route contains id
    const url = this.router.url;
    const regexDeletedProduct: RegExp = new RegExp(`details\/${product.id}`);
    if (regexDeletedProduct.test(url)) {
      await this.router.navigate([``]);
    }
  }

  async importProducts(importerName: string, importParameters: Entry | undefined) {
    const body = {
      importerName: importerName,
      body: importParameters,
    } as Import$Params;

    try {
      await this.service.import(body);
      this.loadProductsForTree();
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
    }
  }
}

