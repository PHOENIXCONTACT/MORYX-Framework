/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, lastValueFrom, Observable } from 'rxjs';
import { ResourceModel, ResourceReferenceModel } from '../api/models';
import { ResourceModificationService } from '../api/services';
import { StrictHttpResponse } from '../api/strict-http-response';
import { CacheResourceService } from './cache-resource.service';
import { ResourceStorageDetails, ResourceStorageObject, SessionService } from './session.service';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { ResourceConstructionParameters } from '../models/ResourceConstructionParameters';
import { toSignal } from '@angular/core/rxjs-interop';

/**
 * This service tracks and manages the resource that is currently edited in the edit view.
 * @service
 */
@Injectable({
  providedIn: 'root',
})
export class EditResourceService {
  private readonly resourceModificationService = inject(ResourceModificationService);
  private readonly cacheResourceService = inject(CacheResourceService);
  private readonly sessionService = inject(SessionService);
  private readonly snackbarService = inject(SnackbarService);

  public readonly edit$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private readonly resource: BehaviorSubject<ResourceModel | undefined> = new BehaviorSubject<ResourceModel | undefined>(
    undefined
  );
  public activeResource = toSignal(this.resource);
  public editingUnsavedResource: boolean = false;
  TranslationConstants = TranslationConstants;
  
  public setResource(resource: ResourceModel | undefined) {
    this.resource.next(resource);
  }

  /**
   * Updates the active resource, e.g. with new property values, pushing the @param resource on the subject. 
   */
  public updateActiveResource(resource: ResourceModel) {
    if (this.resource.value && this.resource.value?.id !== resource.id)
      throw new Error('Trying to update the active resource with a different resource.');
    this.resource.next(resource);
  }

  public resetEditor() {
    this.edit$.next(false);
    this.editingUnsavedResource = false;
    this.resource.next(undefined);
  }

  // ToDo: Call in on destroy
  public stashResource() {
    if (!this.resource.value) return;

    this.sessionService.setWipResource(this.resource.value, <ResourceStorageDetails>{
      createNewResource: this.editingUnsavedResource,
    });
  }

  public setResourceFromStorage(resourceStorageObject: ResourceStorageObject) {
    this.editingUnsavedResource = resourceStorageObject.details.createNewResource;
    this.resource.next(resourceStorageObject.resource);
    this.edit$.next(true);
  }

  public async addNewResource(parameters: ResourceConstructionParameters, parent: ResourceModel | undefined): Promise<ResourceModel | undefined> {
    const resource = await lastValueFrom(this.resourceModificationService
      .constructWithParameters({
        type: parameters.name,
        method: parameters?.method?.name ?? undefined,
        body: parameters.method?.parameters,
      }))
      .catch(async (e: HttpErrorResponse) => await this.snackbarService.handleError(e));
    if (!resource) return;

    this.editingUnsavedResource = resource.id === 0;

    // When the resource was already save, other resources might also be
    if (!this.editingUnsavedResource){
      await this.cacheResourceService.loadResources();
    } 

    if (parent) {
      this.assignReferences(resource, parent);
    }

    this.resource.next(resource);
    this.edit$.next(true);
    return resource;
  }

  public onEdit() {
    this.edit$.next(true);
  }

  public async onSave() {
    const resourceModel = this.resource.getValue();
    if (!resourceModel) return;

    if (resourceModel.properties) 
    {
      PrototypeToEntryConverter.convertToEntry(resourceModel.properties);
    }

    if (this.editingUnsavedResource) 
    {
      await lastValueFrom(this.resourceModificationService.save$Response({body: resourceModel}))
        .then(async response => await this.handleSaveResponse(response))
        .catch(async e => await this.snackbarService.handleError(e));
    }
    else
    {
      await lastValueFrom(this.resourceModificationService.update$Response({id: resourceModel.id!, body: resourceModel}))
        .then(async response => await this.handleUpdateResponse(response))
        .catch(async e => await this.snackbarService.handleError(e));
    }
  }

  async handleUpdateResponse(response: StrictHttpResponse<ResourceModel>) {
    await this.cacheResourceService.loadResources();
    this.resource.next(response.body);
    this.edit$.next(false);
  }

  async handleSaveResponse(response: StrictHttpResponse<ResourceModel>) {
    // load all resources in order to also find resources, which were created automatically in the backend
    // ToDo: Handing over the event through both services seems suboptimal, violates the SR principle for this method.
    await this.cacheResourceService.loadResources();
    const resourceModel = response.body;
    this.editingUnsavedResource = false;
    this.edit$.next(false);
    this.resource.next(resourceModel);
  }

  async onCancel() {
    const resourceId = this.activeResource()?.id;
    if (!resourceId) {
      this.resetEditor();
      return;
    }
    this.edit$.next(false);
    const resource = await lastValueFrom(this.resourceModificationService.getDetails({id: resourceId}));
  }

  private assignReferences(resource: ResourceModel, parent: ResourceModel) {
    const referenceToParent = resource.references?.find(r => r.name == 'Parent');
    if (referenceToParent) referenceToParent.targets = [parent] as ResourceModel[];
    else
      resource.references?.push({
        name: 'Parent',
        targets: [parent] as ResourceModel[],
      } as ResourceReferenceModel);
  }
}

