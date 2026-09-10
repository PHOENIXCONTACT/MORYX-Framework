/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { ProductModel } from '../api/models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly PRODUCT_TREE: string = 'product-tree';
  private readonly PRODUCT_TREE_HIERARCHY: string = 'product-tree-hierarchy';
  private readonly WIP_PRODUCT: string = 'wip-product';

  pushWipProduct(product: ProductModel, details: ProductStorageDetails) {
    const productStorageObject: ProductStorageObject = {product: product, details: details};
    sessionStorage.setItem(this.WIP_PRODUCT, JSON.stringify(productStorageObject));
  }

  getWipProduct(): ProductStorageObject | undefined {
    const item = sessionStorage.getItem(this.WIP_PRODUCT);
    return item ? JSON.parse(item) : undefined;
  }

  popWipProduct(): ProductStorageObject | undefined {
    const product = this.getWipProduct();
    sessionStorage.removeItem(this.WIP_PRODUCT);
    return product;
  }

  getProductTreeHierarchy(): boolean {
    const hierarchic = sessionStorage.getItem(this.PRODUCT_TREE_HIERARCHY);
    if(!hierarchic) {
      return false;
    }
    return JSON.parse(hierarchic);
  }

  setProductTreeHierarchy(hierarchic: boolean) {
    sessionStorage.setItem(this.PRODUCT_TREE_HIERARCHY, hierarchic ? true.toString() : false.toString());
  }

  storeProductTreeExpansion(expandedNames: string[]) {
    sessionStorage.setItem(this.PRODUCT_TREE, expandedNames.join(','));
  }

  getExpandedNodeNames(): string[] {
    const stored = sessionStorage.getItem(this.PRODUCT_TREE);
    return stored ? stored.split(',').filter(Boolean) : [];
  }
}

export interface ProductStorageObject {
  product: ProductModel;
  details: ProductStorageDetails;
}

export interface ProductStorageDetails {
  currentRecipeNumber: number;
  maximumAlreadySavedRecipeId: number;
  currentPartId: number;
  maximumAlreadySavedPartId: number;
}
