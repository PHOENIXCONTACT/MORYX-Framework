import { FlatTreeControl } from '@angular/cdk/tree';
import { Injectable } from '@angular/core';
import { ProductModel } from '../api/models';
import { FlatNode } from '../app.component';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly PRODUCT_TREE: string = 'product-tree';
  private readonly PRODUCT_TREE_HIERARCHY: string = 'product-tree-hierarchy';
  private readonly WIP_PRODUCT: string = 'wip-product';

  constructor() { }

  setWipProduct(product: ProductModel, details: ProductStorageDetails) {
    const productStorageObject: ProductStorageObject = {product: product, details: details};
    sessionStorage.setItem(this.WIP_PRODUCT, JSON.stringify(productStorageObject));
  }

  getWipProduct(): ProductStorageObject | undefined {
    const item = sessionStorage.getItem(this.WIP_PRODUCT);
    return item ? JSON.parse(item) : undefined;
  }

  removeWipProduct() {
    sessionStorage.removeItem(this.WIP_PRODUCT);
  }

  getProductTreeHierarchy(): boolean {
    const hierarchic = sessionStorage.getItem(this.PRODUCT_TREE_HIERARCHY);
    if(!hierarchic)
      return false;
    return JSON.parse(hierarchic);
  }

  setProductTreeHierarchy(hierarchic: boolean) {
    sessionStorage.setItem(this.PRODUCT_TREE_HIERARCHY, hierarchic ? true.toString() : false.toString());
  }

  saveProductTreeExpansion(node: FlatNode, expanded: boolean) {
    let expandedNodesString = "";
    const expandedNodes = sessionStorage.getItem(this.PRODUCT_TREE);
    let expandedNodesArray = expandedNodes ? expandedNodes.split(',') : [];
    const nodeName = node.name;

    if (expanded && !expandedNodesArray.includes(nodeName))
      expandedNodesString = expandedNodes ? expandedNodes + ',' + nodeName : nodeName;
    else if (!expanded && expandedNodesArray.includes(nodeName)) {
      const index = expandedNodesArray.indexOf(nodeName, 0);
      if (index > -1) {
        expandedNodesArray.splice(index, 1);
        for (let id of expandedNodesArray) {
          expandedNodesString += id + ',';
        }
        expandedNodesString = expandedNodesString.slice(0, -1);
      }
    } 
    
    sessionStorage.setItem(this.PRODUCT_TREE, expandedNodesString);
  }

  expandNodesAccordingToStorage(treeControl: FlatTreeControl<FlatNode, FlatNode>) {
    const expandedNodes = sessionStorage.getItem(this.PRODUCT_TREE);
    if (!expandedNodes) return;
  
    const expandedNodesArray = expandedNodes.split(',');
    for (let node of treeControl.dataNodes) {
      if (expandedNodesArray.includes(node.name)) {
        treeControl.expand(node);
      }
    }
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