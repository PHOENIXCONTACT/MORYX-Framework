/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { ResourceModel } from '../api/models';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private readonly RESOURCE_TREE: string = 'resource-tree';
  private readonly WIP_RESOURCE: string = 'wip-resource';

  storeTreeState(expandedIds: number[]) {
    sessionStorage.setItem(this.RESOURCE_TREE, expandedIds.join(','));
  }

  getExpandedIds(): number[] {
    const stored = sessionStorage.getItem(this.RESOURCE_TREE);
    return stored ? stored.split(',').filter(Boolean).map(Number) : [];
  }

  setWipResource(resource: ResourceModel, details: ResourceStorageDetails) {
    const resourceStorageObject: ResourceStorageObject = { resource: resource, details: details };
    sessionStorage.setItem(this.WIP_RESOURCE, JSON.stringify(resourceStorageObject));
  }

  getWipResource(): ResourceStorageObject | undefined {
    const item = sessionStorage.getItem(this.WIP_RESOURCE);
    return item ? JSON.parse(item) : undefined;
  }

  removeWipResource(): ResourceStorageObject | undefined {
    const product = this.getWipResource();
    sessionStorage.removeItem(this.WIP_RESOURCE);
    return product;
  }
}

export interface ResourceStorageObject {
  resource: ResourceModel;
  details: ResourceStorageDetails;
}

export interface ResourceStorageDetails {
  createNewResource: boolean;
}

