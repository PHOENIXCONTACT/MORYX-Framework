/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject } from 'rxjs';
import { ReferenceValue, ResourceModel, ResourceTypeModel } from '../api/models';
import { ResourceModificationService } from '../api/services';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import '../extensions/observable.extensions';

/**
 * This service handles the set of existing resources and resource types
 * @service
 */
@Injectable({
  providedIn: 'root',
})
export class CacheResourceService {
  TranslationConstants = TranslationConstants;
  private readonly ChildReferenceName = 'Children';

  rootType: ResourceTypeModel | undefined;
  flatTypes: ResourceTypeModel[] | undefined;
  resources: BehaviorSubject<ResourceModel[] | undefined> = new BehaviorSubject<ResourceModel[] | undefined>(undefined);
  flatResources: BehaviorSubject<ResourceModel[] | undefined> = new BehaviorSubject<ResourceModel[] | undefined>(
    undefined
  );

  constructor(
    private resourceModification: ResourceModificationService,
    public translate: TranslateService,
    private snackbarService: SnackbarService
  ) {
    this.resources.subscribe(resources => this.pushFlattenedResources(resources));
  }

  private pushFlattenedResources(resources: ResourceModel[] | undefined) {
    var flattendResources = [] as ResourceModel[];
    if (resources) resources.forEach(r => this.collectflattenedResources(r, flattendResources));

    this.flatResources.next(flattendResources);
  }

  private collectflattenedResources(root: ResourceModel, flattendResources: ResourceModel[]) {
    if (flattendResources?.find(r => r.id === root.id)) return;
    flattendResources.push(root);
    root.references
      ?.find(ref => ref.name === this.ChildReferenceName)
      ?.targets?.forEach(r => this.collectflattenedResources(r, flattendResources));
  }

  removeResource(resource: ResourceModel) {
    var newResources = this.resources.getValue() ?? [];

    //Handle children's references
    var childReferences = resource.references?.find(r => r.name === this.ChildReferenceName)?.targets;
    childReferences?.forEach(c => newResources.push(c));

    //Handle parent's reference
    var parent = this.flatResources
      .getValue()
      ?.find(p =>
        p.references?.find(r => r.name === this.ChildReferenceName)?.targets?.find(r => r.id === resource.id)
      );
    if (parent) {
      this.removeChildFromParent(parent, resource);
      this.resources.next(newResources);
    } else {
      this.resources.next(newResources.filter(r => r.id != resource.id));
    }
  }

  private removeChildFromParent(parent: ResourceModel, child: ResourceModel) {
    var childrenReferences = parent?.references?.find(ref => ref.name === this.ChildReferenceName);
    if (childrenReferences) childrenReferences.targets = childrenReferences.targets?.filter(t => t.id != child.id);
  }

  async loadResources() {
    await this.resourceModification
      .getTypeTree()
      .toAsync()
      .then(rootType => {
        this.rootType = rootType;
        this.flatTypes = [];
        this.collectflattenedTypes(rootType, this.flatTypes);
      })
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err));

    await this.resourceModification
      .getResources({
        body: {
          referenceCondition: {
            name: 'Parent',
            valueConstraint: ReferenceValue.NullOrEmpty,
          },
          referenceRecursion: true,
          includedReferences: [{ name: this.ChildReferenceName }],
        },
      })
      .toAsync()
      .then(resources => this.resources.next(resources))
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err));
  }

  private collectflattenedTypes(root: ResourceTypeModel, flattendTypes: ResourceTypeModel[]) {
    if (flattendTypes?.find(t => t.name === root.name)) return;
    flattendTypes.push(root);
    root.derivedTypes?.forEach(t => this.collectflattenedTypes(t, flattendTypes));
  }
}

