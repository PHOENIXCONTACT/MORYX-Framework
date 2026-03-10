/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { formatNumber } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { PrototypeToEntryConverter } from "@moryx/ngx-web-framework/entry-editor";
import { BehaviorSubject } from "rxjs";
import { map } from "rxjs/operators";
import { ProductModel, RecipeModel } from "../api/models";
import { ProductManagementService } from "../api/services/product-management.service";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { DuplicateProductInfos } from "../models/DuplicateProductInfos";
import { CacheProductsService } from "./cache-products.service";
import { ProductStorageObject } from "./session.service";
import { toSignal } from "@angular/core/rxjs-interop";

@Injectable({
  providedIn: "root",
})
export class EditProductsService {
  private productManagementService = inject(ProductManagementService);
  private router = inject(Router);
  private cacheProductsService = inject(CacheProductsService);
  private activatedRoute = inject(ActivatedRoute);
  private snackbarService = inject(SnackbarService);

  public edit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private currentProduct: BehaviorSubject<ProductModel | undefined> = new BehaviorSubject<ProductModel | undefined>(undefined);
  public currentProduct$ = this.currentProduct.asObservable();
  public currentProductId = toSignal(this.currentProduct$.pipe(map((p) => p?.id)));

  private references: BehaviorSubject<ProductModel[] | undefined> = new BehaviorSubject<ProductModel[] | undefined>(undefined);
  public references$ = this.references.asObservable();

  public currentRecipeNumber: number = 0;
  maximumAlreadySavedRecipeId: number = 0;

  public currentPartId: number = 0;
  maximumAlreadySavedPartId: number = 0;
  TranslationConstants = TranslationConstants;

  setProductFromStorage(productStorageObject: ProductStorageObject) {
    this.currentPartId = productStorageObject.details.currentPartId;
    this.currentRecipeNumber = productStorageObject.details.currentRecipeNumber;
    this.maximumAlreadySavedPartId = productStorageObject.details.maximumAlreadySavedPartId;
    this.maximumAlreadySavedRecipeId = productStorageObject.details.maximumAlreadySavedRecipeId;
    this.currentProduct.next(productStorageObject.product);

    this.edit$.next(true);
  }

  setProduct(product: ProductModel | undefined) {
    this.currentProduct.next(product);
  }

  resetProduct() {
    this.currentProduct.next(undefined);
    this.references.next(undefined);
  }

  setReferences(references: ProductModel[] | undefined) {
    this.references.next(references);
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

    this.edit$.next(true);
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

    this.productManagementService
      .updateType({id: productModel.id, body: productModel})
      .subscribe((result) => {
        if (result !== productModel?.id) return;

        this.cacheProductsService.loadProductsForTree();
        this.productManagementService.getTypeById({id: result}).subscribe({
          next: (p) => {
            this.currentProduct.next(p);
            this.edit$.next(false);
          },
          error: async (e: HttpErrorResponse) => {
            await this.snackbarService.handleError(e);
          },
        });
      });
  }

  async onCancel() {
    this.edit$.next(false);
    const currentId = this.currentProductId();
    if (!currentId) return;

    await this.productManagementService
      .getTypeById({id: currentId})
      .toAsync()
      .then(product => this.currentProduct.next(product))
      .catch(async (error) => await this.snackbarService.handleError(error));
  }

  onDuplicate(infos: DuplicateProductInfos) {
    if (!infos.revision || !infos.identifier || !infos.product?.id) return;

    const id = infos.product.id;
    const identifier = this.createProductIdentity(
      infos.identifier,
      infos.revision
    );

    this.productManagementService
      .duplicate({id: id, body: `"${identifier}"`})
      .subscribe({
        next: () => {
          this.cacheProductsService.loadProductsForTree();
          // ToDo: Verify why recipe regex and why different navigations
          const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
          if (regexSpecificRecipe.test(this.router.url)) {
            this.router
              .navigate(["../../"], {relativeTo: this.activatedRoute})
              .then(() => {
                this.router
                  .navigate([`/details/${id}`]);
              });
          } else {
            this.router
              .navigate([`/details/${id}`]);
          }
        },
        error: async (e: HttpErrorResponse) => {
          await this.snackbarService.handleError(e);
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

