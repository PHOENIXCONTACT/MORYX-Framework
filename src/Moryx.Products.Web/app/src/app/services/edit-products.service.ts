/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { formatNumber } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { computed, inject, Injectable, linkedSignal, signal } from "@angular/core";
import { Router } from "@angular/router";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { PrototypeToEntryConverter } from "@moryx/ngx-web-framework/entry-editor";

import { PartConnector, PartModel, ProductModel, RecipeModel } from "../api/models";
import { ProductManagementService } from "@api/services/product-management.service";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { DuplicateProductInfos } from "../models/DuplicateProductInfos";
import { CacheProductsService } from "./cache-products.service";
import { ProductStorageObject } from "./session.service";

@Injectable({
  providedIn: "root",
})
export class EditProductsService {
  private productManagementService = inject(ProductManagementService);
  private router = inject(Router);
  private cacheProductsService = inject(CacheProductsService);
  private snackbarService = inject(SnackbarService);

  private readonly _editing = signal<boolean>(false);
  readonly editing = this._editing.asReadonly();

  private readonly _currentProduct = signal<ProductModel | undefined>(undefined);
  readonly currentProduct = this._currentProduct.asReadonly();
  readonly currentProductId = computed(() => this.currentProduct()?.id);

  private readonly _references = signal<ProductModel[] | undefined>(undefined);
  readonly references = this._references.asReadonly();

  private recipe = linkedSignal<ProductModel | undefined, RecipeModel | undefined>({
    source: this.currentProduct,
    computation: (currentProduct, previous) => {
    if (!currentProduct) {
      return undefined;
    }
    return currentProduct.recipes?.find(r => r.id === previous?.value?.id);
  }});
  public currentRecipe = this.recipe.asReadonly();
  public currentRecipeNumber: number = 0;
  maximumAlreadySavedRecipeId: number = 0;

  private partConnector = linkedSignal<ProductModel | undefined, PartConnector | undefined>({
    source: this.currentProduct,
    computation: (currentProduct, previous) => {
    if (!currentProduct) {
      return undefined;
    }
    return currentProduct.parts?.find(c => c.name === previous?.value?.name);
  }});
  public currentPartConnector = this.partConnector.asReadonly();
  private part = linkedSignal<PartConnector | undefined, PartModel | undefined>({
    source: this.partConnector,
    computation: (partConnector, previous) => {
      if (!partConnector) {
        return undefined;
      }
      return partConnector.parts?.find(p => p.id === previous?.value?.id);
    }});
  public currentPart = this.part.asReadonly();
  public currentPartId: number = 0;
  public maximumAlreadySavedPartId: number = 0;
  TranslationConstants = TranslationConstants;

  setProductFromStorage(productStorageObject: ProductStorageObject) {
    this.currentPartId = productStorageObject.details.currentPartId;
    this.currentRecipeNumber = productStorageObject.details.currentRecipeNumber;
    this.maximumAlreadySavedPartId = productStorageObject.details.maximumAlreadySavedPartId;
    this.maximumAlreadySavedRecipeId = productStorageObject.details.maximumAlreadySavedRecipeId;
    this._currentProduct.set(productStorageObject.product);
    this.recipe.set(productStorageObject.product.recipes?.find(r => r.id === productStorageObject.details.currentRecipeNumber));

    this._editing.set(true);
  }

  setProduct(product: ProductModel | undefined) {
    this._currentProduct.set(product);
    this._references.set(undefined);
  }

  updateCurrentProduct(product: ProductModel) {
    const current = this.currentProduct();
    if (Object.is(product, current)) {
      return;
    }
    if (current?.id !== product.id) {
      throw new Error("Tried to update product with id " + product.id + " but current product has id " + current?.id);
    }
    this._currentProduct.set(product);
  }

  resetProduct() {
    this._currentProduct.set(undefined);
    this._references.set(undefined);
  }

  setReferences(references: ProductModel[] | undefined) {
    this._references.set(references);
  }

  onEdit() {
    // Find the maximum id of the recipes of this product
    const currentProduct = this.currentProduct();
    if (currentProduct?.recipes) {
      const recipeIds = currentProduct.recipes
        .map((e) => e.id)
        .filter((val) => typeof val === "number") as number[];
      if (recipeIds.length === 0) {
        this.currentRecipeNumber = 0;
      } else {
        this.currentRecipeNumber = Math.max(...recipeIds);
      }
      this.maximumAlreadySavedRecipeId = this.currentRecipeNumber;
    }

    // Find the maximum id of the part connectors of this product
    if (currentProduct?.parts?.length) {
      const allPartIds = currentProduct.parts
        .flatMap((p) => p.parts)
        .map((e) => e?.id)
        .filter(filterEmpties);
      if (allPartIds.length === 0) {
        this.currentPartId = 0;
      } else {
        this.currentPartId = Math.max(...allPartIds);
      }
      this.maximumAlreadySavedPartId = this.currentPartId;
    }

    this._editing.set(true);
  }

  async onSave() {
    const productModel = this.currentProduct();
    if (!productModel || productModel.id === undefined) {
      return;
    }

    // Replace CREATED<number> with CREATED in collections
    if (productModel.properties) {
      PrototypeToEntryConverter.convertToEntry(productModel.properties);
    }

    for (const recipe of productModel.recipes ?? []) {
      // Recipes with the id 0 will be created and saved as new recipes in the backend
      if (!recipe.id || recipe.id > this.maximumAlreadySavedRecipeId) {
        recipe.id = 0;
      }

      if (recipe.properties) {
        PrototypeToEntryConverter.convertToEntry(recipe.properties);
      }
    }

    for (const partConnector of productModel?.parts ?? []) {
      for (const part of partConnector.parts ?? []) {
        if (!part.id || part.id > this.maximumAlreadySavedPartId) {
          part.id = 0;
        }

        if (part.properties) {
          PrototypeToEntryConverter.convertToEntry(part.properties);
        }
      }
    }

    let updated: ProductModel = {};
    try {
      await this.productManagementService.updateType({id: productModel.id, body: productModel});
      this.cacheProductsService.loadProductsForTree();
      updated = await this.productManagementService.getTypeById({id: productModel.id});
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }

    this._currentProduct.set(updated);
    this._editing.set(false);
  }

  async onCancel() {
    this._editing.set(false);
    const to = this.router.url;
    await this.router.navigate(['/cancel'], { queryParams: { to: encodeURIComponent(to) }, replaceUrl: true, });
  }

  async onDuplicate(infos: DuplicateProductInfos) {
    if (!infos.revision || !infos.identifier || !infos.product?.id) {
      return;
    }

    const id = infos.product.id;
    const identifier = this.createProductIdentity(
      infos.identifier,
      infos.revision
    );

    try {
      const product = await this.productManagementService.duplicate({id: id, body: `"${identifier}"`});
      this.cacheProductsService.loadProductsForTree();
      this.router.navigate(['details', product.id]);
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
    }
  }

  createProductIdentity(
    identifier: string | undefined | null,
    revision: number | undefined
  ): string {
    if (!identifier || revision === undefined) {
      return "";
    }

    const formatedRevision = formatNumber(revision, "en-US", "2.0-0");
    const identity = identifier + "-" + formatedRevision;

    return identity;
  }

  // Creates the name of a product with <identifier>-<revision> <name>. For example: 1234567-01 product
  createProductNameWithIdentity(product: ProductModel | undefined) {
    if (!product) {
      return "";
    }
    return this.createProductIdentity(product.identifier, product.revision) +
      " " +  product.name;
  }

  addRecipe(recipe: RecipeModel) {const currentProduct = this.currentProduct();
    if (!currentProduct) {
      throw new Error("Invalid State: No current product set");
    }
    currentProduct.recipes!.push(recipe);
    this._currentProduct.set({...currentProduct, recipes: [...currentProduct.recipes!]});
  }

  setRecipe(recipe: RecipeModel | undefined) {
    this.recipe.set(recipe);
  }

  updateCurrentRecipe(recipe: RecipeModel) {
    const currentRecipe = this.recipe();
    if (Object.is(recipe, currentRecipe)) {
      return;
    }
    if (currentRecipe?.id !== recipe.id) {
      throw new Error("Invalid State: Tried to update recipe with id " + recipe.id + " but current recipe has id " + currentRecipe?.id);
    }
    const currentProduct = this.currentProduct();
    if (!currentProduct) {
      throw new Error("Invalid State: No current product set");
    }
    const recipeIndex = currentProduct.recipes?.findIndex(r => r.id === recipe.id);
    if (recipeIndex === undefined || recipeIndex < 0) {
      throw new Error("Invalid State: Tried to update recipe with id " + recipe.id + " but it was not found in current product");
    }
    this.recipe.set(recipe);
    this._currentProduct.update(product => {
      const recipes = [...product!.recipes!];
      recipes[recipeIndex] = recipe;
      return {...product!, recipes};
    });
  }

  removeRecipe(recipe: RecipeModel) {
    const currentProduct = this.currentProduct();
    if (!currentProduct?.recipes) {
      return;
    }
    currentProduct.recipes = currentProduct.recipes.filter(
      (r) => r.id !== recipe.id
    );
    this._currentProduct.set(currentProduct);
  }

  setPartConnector(partConnector: PartConnector | undefined) {
    this.partConnector.set(partConnector);
  }

  setPart(part: PartModel | undefined) {
    this.part.set(part);
  }

  addPartToConnector(newPart: PartModel) {
    const product = this.currentProduct();
    if (!product) {
      throw new Error("Invalid State: No current product set");
    }

    const connector = this.partConnector();
    if (!connector) {
      throw new Error("Invalid State: No part connector selected");
    }

    // Add new Part to PartLink
    newPart.id = ++this.currentPartId;
    if (connector.isCollection) {
      connector.parts = [...connector.parts!, newPart];
    }
    else {
      connector.parts = [newPart];
    }

    const updatedConnectors = product.parts?.map(c => c.name === connector.name ? {...connector} : c) ?? [];
    this._currentProduct.set({...product, parts: updatedConnectors});
    return newPart;
  }

  removePartFromConnector() {
    const product = this.currentProduct();
    if (!product) {
      throw new Error("Invalid State: No current product set");
    }

    const connector = this.partConnector();
    if (!connector) {
      throw new Error("Invalid State: No part connector selected");
    }
    const part = this.part();

    if (connector?.isCollection) {
      if (!part) {
        throw new Error("Invalid State: No part selected");
      }
      connector.parts = connector.parts?.filter((p) => p.id !== part.id);
    } else {
      connector.parts = [] as PartModel[];
    }
    const updatedConnectors = product.parts?.map(c => c.name === connector.name ? {...connector} : c) ?? [];
    this._currentProduct.set({...product, parts: updatedConnectors});
  }
}

// function to filter all null and undefined values from an array
function filterEmpties<TValue>(
  value: TValue | null | undefined
): value is TValue {
  return value !== null && value !== undefined;
}

