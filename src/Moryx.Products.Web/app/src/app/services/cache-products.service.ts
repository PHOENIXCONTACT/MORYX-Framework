/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { BehaviorSubject } from 'rxjs';
import {
  ProductDefinitionModel,
  ProductImporter,
  WorkplanModel,
  ProductModel,
  RecipeDefinitionModel,
  RevisionFilter,
  Selector,
  ProductQuery,
} from '../api/models';
import { ProductManagementService } from '../api/services/product-management.service';
import { FilterOptions } from '../models/FilterOptions';
import { WorkplanService } from '../api/services/workplan.service';
import '../extensions/observable.extensions';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class CacheProductsService {
  definitions: BehaviorSubject<ProductDefinitionModel[] | undefined> = new BehaviorSubject<ProductDefinitionModel[] | undefined>(undefined);
  productsShownInTheTree: BehaviorSubject<ProductModel[] | undefined> = new BehaviorSubject<ProductModel[] | undefined>(undefined);
  importers: BehaviorSubject<ProductImporter[] | undefined> = new BehaviorSubject<ProductImporter[] | undefined>(undefined);
  recipeDefitions: BehaviorSubject<RecipeDefinitionModel[] | undefined> = new BehaviorSubject<RecipeDefinitionModel[] | undefined>(undefined);
  selected: ProductModel[] | undefined;
  workplans: BehaviorSubject<WorkplanModel[] | undefined> = new BehaviorSubject<WorkplanModel[] | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  public filterOptions: FilterOptions = {
    name: '',
    identifier: '',
    revision: RevisionFilter.Latest,
    selector: Selector.Direct,
  } as FilterOptions;

  constructor(
    private service: ProductManagementService,
    private workplanService: WorkplanService,
    private router: Router,
    private snackbarService: SnackbarService,
    public translate: TranslateService
  ) {
  }

  loadConfiguration() {
    this.service.getProductCustomization().subscribe({
      next: (configuration) => {
        if (configuration.importers !== null)
          this.importers.next(configuration.importers);
        if (configuration.productTypes !== null)
          this.definitions.next(configuration.productTypes);
        if (configuration.recipeTypes !== null)
          this.recipeDefitions.next(configuration.recipeTypes);
      },
      error: async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
      },
    });

    this.workplanService.getAllWorkplans().subscribe({
      next: (workplans) => {
        this.workplans.next(workplans);
      },
      error: async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
      },
    });
  }

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

    this.service.getTypes({body: body}).subscribe({
      next: (products) => {
        if (products !== null) this.productsShownInTheTree.next(products);
      },
      error: async (err) => await this.showErrorSnackbar(),
    });
  }

  private async showErrorSnackbar() {
    const translations = await this.translate
      .get([
        TranslationConstants.APP.FAILED_LOADING,
        TranslationConstants.DISMISS,
      ])
      .toAsync();
    await this.snackbarService.showError(
      translations[TranslationConstants.APP.FAILED_LOADING]
    );
  }

  resetfilter() {
    this.filterOptions.identifier = '';
    this.filterOptions.name = '';
    this.filterOptions.revision = RevisionFilter.Latest;
    this.filterOptions.selector = Selector.Direct;
    this.loadProductsForTree();
  }

  async deleteProduct(product: ProductModel) {
    if (!product.id) return;

    let success: boolean = false;
    await this.service
      .deleteType({id: product.id})
      .toAsync()
      .then((res) => (success = res))
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    if (!success) return;

    let newProductsForTree = this.productsShownInTheTree.getValue() ?? [];
    //Check if an older revision exists and, if yes, show that one
    newProductsForTree = newProductsForTree.filter((r) => r.id != product.id);
    const body = {
      identifier: product.identifier,
      revisionFilter: RevisionFilter.Latest,
    };

    await this.service
      .getTypes({body: body})
      .toAsync()
      .then((results) => {
        if (results && results[0]) {
          const existingRevision = newProductsForTree.find(p => p.id === results[0].id);
          if (!existingRevision) {
            const otherRevision = results[0];
            newProductsForTree.push(otherRevision);
          }
        }
        this.productsShownInTheTree.next(newProductsForTree);
      })
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );

    //check if current route contains id
    const url = this.router.url;
    const regexDeletedProduct: RegExp = new RegExp(`details\/${product.id}`);
    if (regexDeletedProduct.test(url)) {
      this.router.navigate([``]);
    }
  }

  importProducts(importerName: string, importParameters: Entry | undefined) {
    return new Promise<boolean>((resolve) => {
      let body = {} as { importerName: string; body?: Entry | undefined };
      if (importParameters) {
        body = {
          importerName: importerName,
          body: importParameters,
        };
      } else {
        body = {
          importerName: importerName,
        };
      }

      this.service.import(body).subscribe({
        next: (products) => {
          let newProductsOfTree = this.productsShownInTheTree.value ?? [];
          for (let product of products) {
            newProductsOfTree.push(product);
          }
          this.productsShownInTheTree.next(newProductsOfTree);
        },
        error: async (e: HttpErrorResponse) => {
          await this.snackbarService.handleError(e);
        },
      });

      resolve(true);
    });
  }

  //TODO: move this function to EntryEditor package
  cloneEntry(prototype: Entry): Entry {
    const entry = {...prototype};
    entry.validation = {...prototype.validation};
    entry.value = {...prototype.value};
    entry.description = `${prototype.description}`;
    entry.displayName = `${prototype.displayName}`;
    if (prototype.subEntries) {
      entry.subEntries = [] as Entry[];
      for (let i = 0; i < prototype.subEntries?.length; i++) {
        entry.subEntries[i] = this.cloneEntry(prototype.subEntries[i]);
      }
    }
    if (prototype.prototypes) {
      entry.prototypes = [] as Entry[];
      for (let i = 0; i < prototype.prototypes?.length; i++) {
        entry.prototypes[i] = this.cloneEntry(prototype.prototypes[i]);
      }
    }
    return entry;
  }
}

