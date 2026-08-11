/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { ReferenceValue, ResourceModel, ResourceTypeModel } from '../api/models';
import { ResourceModificationService } from '../api/services';
import { TranslationConstants } from '../extensions/translation-constants';


/**
 * This service handles the set of existing resources and resource types
 * @service
 */
@Injectable({
  providedIn: 'root',
})
export class CacheResourceService {
  private resourceModificationService = inject(ResourceModificationService);
  private snackbarService = inject(SnackbarService);

  TranslationConstants = TranslationConstants;
  private readonly ChildReferenceName = 'Children';

  rootType: ResourceTypeModel | undefined;
  flatTypes: ResourceTypeModel[] | undefined;
  private readonly _resources = signal<ResourceModel[] | undefined>(undefined);
  readonly resources = this._resources.asReadonly();
  flatResources = computed<ResourceModel[]>(() => {
    const resources = this.resources();
    const flattened: ResourceModel[] = [];
    if (resources) {
      resources.forEach(r => this.collectflattenedResources(r, flattened));
    }
    return flattened;
  });

  private collectflattenedResources(root: ResourceModel, flattendResources: ResourceModel[]) {
    if (flattendResources?.find(r => r.id === root.id)) {
      return;
    }
    flattendResources.push(root);
    root.references
      ?.find(ref => ref.name === this.ChildReferenceName)
      ?.targets?.forEach(r => this.collectflattenedResources(r, flattendResources));
  }

  removeResource(resource: ResourceModel) {
    const newResources = this.resources() ?? [];

    //Handle children's references
    const childReferences = resource.references?.find(r => r.name === this.ChildReferenceName)?.targets;
    childReferences?.forEach(c => newResources.push(c));

    //Handle parent's reference
    const parent = this.flatResources()
      ?.find(p =>
        p.references?.find(r => r.name === this.ChildReferenceName)?.targets?.find(r => r.id === resource.id)
      );
    if (parent) {
      this.removeChildFromParent(parent, resource);
      this._resources.set(newResources);
    } else {
      this._resources.set(newResources.filter(r => r.id != resource.id));
    }
  }

  private removeChildFromParent(parent: ResourceModel, child: ResourceModel) {
    const childrenReferences = parent?.references?.find(ref => ref.name === this.ChildReferenceName);
    if (childrenReferences) {
      childrenReferences.targets = childrenReferences.targets?.filter(t => t.id != child.id);
    }
  }

  async loadResources() {
    await this.resourceModificationService
      .getTypeTree()
      .then(rootType => {
        this.rootType = rootType;
        this.flatTypes = [];
        this.collectflattenedTypes(rootType, this.flatTypes);
      })
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));

    await this.resourceModificationService
      .getResources({
        body: {
          referenceCondition: {
            name: 'Parent',
            valueConstraint: ReferenceValue.NullOrEmpty,
          },
          referenceRecursion: true,
          includedReferences: [{name: this.ChildReferenceName}],
        },
      })
      .then(resources => this._resources.set(resources))
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  private collectflattenedTypes(root: ResourceTypeModel, flattendTypes: ResourceTypeModel[]) {
    if (flattendTypes?.find(t => t.name === root.name)) {
      return;
    }
    flattendTypes.push(root);
    root.derivedTypes?.forEach(t => this.collectflattenedTypes(t, flattendTypes));
  }
}

