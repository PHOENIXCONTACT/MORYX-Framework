/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { formatNumber } from "@angular/common";
import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { Inject, Injectable, LOCALE_ID } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import {
  MoryxSnackbarService,
  PrototypeToEntryConverter,
} from "@moryx/ngx-web-framework";
import { TranslateService } from "@ngx-translate/core";
import { BehaviorSubject } from "rxjs";
import {
  ProductModel,
  ProductQuery,
  RecipeModel,
  RevisionFilter,
  Selector,
} from "../api/models";
import { ProductManagementService } from "../api/services/product-management.service";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { DuplicateProductInfos } from "../models/DuplicateProductInfos";
import { CacheProductsService } from "./cache-products.service";
import { SessionService } from "./session.service";

@Injectable({
  providedIn: "root",
})
export class EditProductsService {
  public edit: boolean = false;
  public currentProduct: BehaviorSubject<ProductModel | undefined> =
    new BehaviorSubject<ProductModel | undefined>(undefined);
  public references: BehaviorSubject<ProductModel[] | undefined> =
    new BehaviorSubject<ProductModel[] | undefined>(undefined);
  public currentRecipeNumber: number = 0;
  maximumAlreadySavedRecipeId: number = 0;
  public currentPartId: number = 0;
  maximumAlreadySavedPartId: number = 0;
  currentProductId: number = 0;
  TranslationConstants = TranslationConstants;
  constructor(
    private managementService: ProductManagementService,
    private router: Router,
    private cacheService: CacheProductsService,
    private sessionService: SessionService,
    private route: ActivatedRoute,
    @Inject(LOCALE_ID) public locale: string,
    private moryxSnackbar: MoryxSnackbarService,
    private translate: TranslateService
  ) {}

  loadFromStorage() {
    const productStorageObject = this.sessionService.getWipProduct();
    if (productStorageObject) {
      this.currentProductId = productStorageObject.product.id!;
      this.currentPartId = productStorageObject.details.currentPartId;
      this.currentRecipeNumber =
        productStorageObject.details.currentRecipeNumber;
      this.maximumAlreadySavedPartId =
        productStorageObject.details.maximumAlreadySavedPartId;
      this.maximumAlreadySavedRecipeId =
        productStorageObject.details.maximumAlreadySavedRecipeId;
      this.currentProduct.next(productStorageObject.product);
    }
  }

  loadProduct() {
    let id = 0;
    const navigation = this.router.currentNavigation();
    if (
      navigation &&
      (navigation?.finalUrl?.root.children["primary"]?.segments?.length ??
        0 > 1)
    )
      id = Number(
        navigation.finalUrl?.root.children["primary"].segments[1].toString()
      );
    else {
      const url = this.router.url;
      const regexId: RegExp = /(details\/\d*)/;
      if (!regexId.test(url)) {
        this.currentProduct.next(undefined);
        return;
      }
      id = Number(url.split("/")[2]);
    }

    this.currentProductId = id;

    if (id === 0) {
      this.unloadProduct();
      return;
    }
    this.loadProductById(id);
  }

  loadProductById(id: number) {
    this.managementService.getTypeById({ id: id }).subscribe({
      next: (product) => {
        this.currentProduct.next(product);
        this.getReferencesOfCurrentProduct();
      },
      error: async (e: HttpErrorResponse) => {
        await this.handleLoadError(e);
        this.unloadProduct();
      },
    });
  }

  private async handleLoadError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // Unknown errors occur most commonly when the server is not reachable.
      // That is handled somewhere else, so there is no need to show that here.
      return;
    }

    if (error.error?.title !== undefined) {
      await this.moryxSnackbar.showError(error.error?.title);
    } else {
      await this.moryxSnackbar.handleError(error);
    }
  }

  unloadProduct() {
    this.currentProduct.next(undefined);
    this.router.navigate([""]);
  }

  private getReferencesOfCurrentProduct() {
    const product = this.currentProduct.value;
    if (!product) return;

    const body = <ProductQuery>{
      includeDeleted: false,
      identifier: product.identifier,
      revision: product.revision,
      revisionFilter: RevisionFilter.Specific,
      selector: Selector.Parent,
    };
    this.managementService.getTypes({ body: body }).subscribe({
      next: (references) => {
        this.references.next(references);
      },
      error: async (e: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(e);
      },
    });
  }

  onEdit() {
    // Find the maximum id of the recipes of this product
    const currentProduct = this.currentProduct.value;
    if (currentProduct?.recipes) {
      const recipeIds = currentProduct.recipes
        .map((e) => e.id)
        .filter((val) => typeof val === "number") as number[];
      if (recipeIds.length === 0) this.currentRecipeNumber = 0;
      else this.currentRecipeNumber = Math.max(...recipeIds);
      this.maximumAlreadySavedRecipeId = this.currentRecipeNumber;
    }

    // Find the maximum id of the part connectors of this product
    if (currentProduct?.parts?.length) {
      let allPartIds = currentProduct.parts
        .flatMap((p) => p.parts)
        .map((e) => e?.id)
        .filter(filterEmpties);
      if (allPartIds.length === 0) this.currentPartId = 0;
      else this.currentPartId = Math.max(...allPartIds);
      this.maximumAlreadySavedPartId = this.currentPartId;
    }

    this.edit = true;
  }

  onSave() {
    const productModel = this.currentProduct.value;
    if (!productModel || productModel.id === undefined) return;

    // Replace CREATED<number> with CREATED in collections
    if (productModel.properties)
      PrototypeToEntryConverter.convertToEntry(productModel.properties);

    if (productModel.recipes) {
      for (let recipe of productModel.recipes) {
        // Recipes with the id 0 will be created and saved as new recipes in the backend
        if (!recipe.id) recipe.id = 0;
        else if (recipe.id > this.maximumAlreadySavedRecipeId) recipe.id = 0;

        if (recipe.properties)
          PrototypeToEntryConverter.convertToEntry(recipe.properties);
      }
    }

    if (productModel?.parts?.length) {
      for (let partConnector of productModel.parts) {
        if (!partConnector.parts?.length) continue;
        for (let part of partConnector.parts) {
          if (!part.id) part.id = 0;
          else if (part.id > this.maximumAlreadySavedPartId) part.id = 0;

          if (part.properties)
            PrototypeToEntryConverter.convertToEntry(part.properties);
        }
      }
    }

    this.managementService
      .updateType({ id: productModel.id, body: productModel })
      .subscribe((result) => {
        if (result !== productModel?.id) return;

        this.cacheService.loadProductsForTree();
        this.managementService.getTypeById({ id: result }).subscribe({
          next: (p) => {
            this.currentProduct.next(p);
            this.edit = false;
          },
          error: async (e: HttpErrorResponse) => {
            await this.moryxSnackbar.handleError(e);
          },
        });
      });
  }

  async onCancel() {
    this.edit = false;
    if (!this.currentProduct.value?.id) return;

    await this.managementService
      .getTypeById({ id: this.currentProduct.value.id })
      .toAsync()
      .then(product => this.currentProduct.next(product))
      .catch(async (error) => await this.moryxSnackbar.handleError(error));
  }

  onDuplicate(infos: DuplicateProductInfos) {
    if (!infos.revision || !infos.identifier || !infos.product?.id) return;

    const identifier = this.createProductIdentity(
      infos.identifier,
      infos.revision
    );

    this.managementService
      .duplicate({ id: infos.product.id, body: `"${identifier}"` })
      .subscribe({
        next: (product) => {
          this.cacheService.loadProductsForTree();
          const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
          if (regexSpecificRecipe.test(this.router.url)) {
            this.router
              .navigate(["../../"], { relativeTo: this.route })
              .then(() => {
                this.router
                  .navigate([`/details/${product.id}`])
                  .then(() => this.loadProduct());
              });
          } else {
            this.router
              .navigate([`/details/${product.id}`])
              .then(() => this.loadProduct());
          }
        },
        error: async (e: HttpErrorResponse) => {
          await this.moryxSnackbar.handleError(e);
        },
      });
  }

  createProductIdentity(
    identifier: string | undefined | null,
    revision: number | undefined
  ): string {
    if (!identifier || revision === undefined) return "";

    const formatedRevision = formatNumber(revision, "en-US", "2.0-0");
    const identity = identifier + "-" + formatedRevision;

    return identity;
  }

  // Creates the name of a product with <identifier>-<revision> <name>. For example: 1234567-01 product
  createProductNameWithIdentity(
    product: ProductModel | undefined,
    shortened: boolean = false,
    maxLength: number = 40
  ) {
    if (!product) return "";
    let productName =
      this.createProductIdentity(product.identifier, product.revision) +
      " " +
      product.name;
    if (shortened && productName.length > maxLength) {
      productName = productName.substring(0, maxLength - 4) + "...";
    }
    return productName;
  }

  addRecipe(recipe: RecipeModel) {
    const currentProduct = this.currentProduct.value;
    currentProduct?.recipes?.push(recipe);
    this.currentProduct.next(currentProduct);
  }

  removeRecipe(recipe: RecipeModel) {
    const currentProduct = this.currentProduct.value;
    if (!currentProduct?.recipes) return;
    currentProduct.recipes = currentProduct.recipes.filter(
      (r) => r.id !== recipe.id
    );
    this.currentProduct.next(currentProduct);
  }
}

// function to filter all null and undefined values from an array
function filterEmpties<TValue>(
  value: TValue | null | undefined
): value is TValue {
  return value !== null && value !== undefined;
}

