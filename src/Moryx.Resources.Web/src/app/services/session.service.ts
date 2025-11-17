import { FlatTreeControl } from '@angular/cdk/tree';
import { Injectable } from '@angular/core';
import { ResourceModel } from '../api/models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly RESOURCE_TREE: string = 'resource-tree';
  private readonly WIP_RESOURCE: string = 'wip-resource';

  constructor() { }

  storeTreeState(treeControl: FlatTreeControl<FlatNode, FlatNode>) {
    const nodes = treeControl.dataNodes.filter(n => treeControl.isExpanded(n)).map(n => n.id);
    const nodesString = nodes.join(",");
    sessionStorage.setItem(this.RESOURCE_TREE, nodesString);
  }

  restoreTreeState(treeControl: FlatTreeControl<FlatNode, FlatNode>) : void {
    const expandedNodes = sessionStorage.getItem(this.RESOURCE_TREE);
    if (!expandedNodes) return;
    
    const expandedNodesArray = expandedNodes.split(',');
    for (let id of expandedNodesArray) {
      const node = treeControl.dataNodes.find(n => String(n.id) === id);
      if(!node) continue;
      treeControl.expand(node);
    }
  }

  setWipResource(resource: ResourceModel, details: ResourceStorageDetails) {
    const resourceStorageObject: ResourceStorageObject = {resource: resource, details: details};
    sessionStorage.setItem(this.WIP_RESOURCE, JSON.stringify(resourceStorageObject));
  }

  getWipResource(): ResourceStorageObject | undefined {
    const item = sessionStorage.getItem(this.WIP_RESOURCE);
    return item ? JSON.parse(item) : undefined;
  }

  removeWipResource() {
    sessionStorage.removeItem(this.WIP_RESOURCE);
  }

}

export interface ResourceStorageObject {
  resource: ResourceModel;
  details: ResourceStorageDetails;
}

export interface ResourceStorageDetails {
  createNewResource: boolean;
}

export interface FlatNode {
  expandable: boolean;
  name: string;
  level: number;
  id: number;
}
