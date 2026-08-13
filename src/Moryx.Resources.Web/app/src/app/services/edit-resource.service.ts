/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';

import { ResourceModel } from '../api/models';
import { ResourceModificationService } from '../api/services';
import { StrictHttpResponse } from '@api/strict-http-response';
import { CacheResourceService } from './cache-resource.service';
import { ResourceStorageDetails, ResourceStorageObject, SessionService } from './session.service';
import { TranslationConstants } from '../translation-constants';
import { HttpErrorResponse } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';

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

  private readonly _editing = signal<boolean>(false);
  readonly editing = this._editing.asReadonly();
  private readonly _activeResource = signal<ResourceModel | undefined>(undefined);
  readonly activeResource = this._activeResource.asReadonly();
  public editingUnsavedResource: boolean = false;
  TranslationConstants = TranslationConstants;

  public setResource(resource: ResourceModel | undefined) {
    this._activeResource.set(resource);
  }

  /**
   * Updates the active resource, e.g. with new property values, pushing the @param resource on the subject.
   */
  public updateActiveResource(resource: ResourceModel) {
    const current = this.activeResource();
    if (current && current.id !== resource.id) {
      throw new Error('Trying to update the active resource with a different resource.');
    }
    this._activeResource.set(resource);
  }

  public resetEditor() {
    this._editing.set(false);
    this.editingUnsavedResource = false;
    this._activeResource.set(undefined);
  }

  public stashResource() {
    const resource = this.activeResource();
    if (!resource) {
      return;
    }

    this.sessionService.setWipResource(resource, <ResourceStorageDetails>{
      createNewResource: this.editingUnsavedResource,
    });
  }

  public setResourceFromStorage(resourceStorageObject: ResourceStorageObject) {
    this.editingUnsavedResource = resourceStorageObject.details.createNewResource;
    this._activeResource.set(resourceStorageObject.resource);
    this._editing.set(true);
  }

  public async registerNewResource(constructed: ResourceModel) {
    this.editingUnsavedResource = constructed.id === 0;
    // When the resource was already save, other resources might also be
    if (!this.editingUnsavedResource){
      await this.cacheResourceService.loadResources();
    }

    this._activeResource.set(constructed);
    this._editing.set(true);
  }

  public onEdit() {
    this._editing.set(true);
  }

  public async onSave() {
    const resourceModel = this.activeResource();
    if (!resourceModel) {
      return;
    }

    if (resourceModel.properties) {
      PrototypeToEntryConverter.convertToEntry(resourceModel.properties);
    }

    if (this.editingUnsavedResource) {
      await this.resourceModificationService.save$Response({body: resourceModel})
        .then(response => this.handleSaveResponse(response))
        .catch(e => this.snackbarService.handleError(e));
    } else {
      await this.resourceModificationService.update$Response({id: resourceModel.id!, body: resourceModel})
        .then(response => this.handleUpdateResponse(response))
        .catch(e => this.snackbarService.handleError(e));
    }
  }

  private async handleUpdateResponse(response: StrictHttpResponse<ResourceModel>) {
    await this.cacheResourceService.loadResources();
    this._activeResource.set(response.body);
    this._editing.set(false);
  }

  private async handleSaveResponse(response: StrictHttpResponse<ResourceModel>) {
    // load all resources in order to also find resources, which were created automatically in the backend
    // ToDo: Handing over the event through both services seems suboptimal, violates the SR principle for this method.
    await this.cacheResourceService.loadResources();
    const resourceModel = response.body;
    this.editingUnsavedResource = false;
    this._editing.set(false);
    this._activeResource.set(resourceModel);
  }

  public async onCancel() {
    const resourceId = this.activeResource()?.id;
    if (!resourceId) {
      this.resetEditor();
      return;
    }
    this._editing.set(false);
    try {
      const resource = await this.resourceModificationService.getDetails({id: resourceId});
      this._activeResource.set(resource);
    }
    catch (e) {
      await this.snackbarService.handleError(e as HttpErrorResponse);
    }
  }
}

