/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { ResourceModel, ResourceReferenceModel } from '../api/models';
import { ResourceModificationService } from '../api/services';
import { StrictHttpResponse } from '../api/strict-http-response';
import { CacheResourceService } from './cache-resource.service';
import { ResourceStorageDetails, SessionService } from './session.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { ResourceConstructionParameters } from '../models/ResourceConstructionParameters';

/**
 * This service tracks and manages the resource that is currently edited in the edit view.
 * @service
 */
@Injectable({
  providedIn: 'root',
})
export class EditResourceService {
  public edit: boolean = false;
  private resource: BehaviorSubject<ResourceModel | undefined> = new BehaviorSubject<ResourceModel | undefined>(
    undefined
  );
  public activeResource$: Observable<ResourceModel | undefined> = this.resource.asObservable();
  public editingUnsavedResource: boolean = false;
  TranslationConstants = TranslationConstants;

  constructor(
    private modificationService: ResourceModificationService,
    private cacheService: CacheResourceService,
    private router: Router,
    private sessionService: SessionService,
    private translate: TranslateService,
    private snackbarService: SnackbarService
  ) { }

  loadResource() {
    var id = 0;

    var navigation = this.router.currentNavigation();
    if (navigation?.finalUrl?.root.children['primary']?.segments?.length)
      id = Number(navigation.finalUrl?.root.children['primary'].segments[1].toString());
    else {
      var url = this.router.url;
      const regexId: RegExp = /(details\/\d*)/;
      if (regexId.test(url)) id = Number(url.split('/')[2]);
    }

    if (id === 0) {
      this.resetEditor();
      return;
    }

    this.modificationService.getDetails({ id: id }).subscribe({
      next: r => {
        this.resource.next(r);
      },
      error: async error => {
        await this.showErrorSnackbar(error);
        this.resetEditor();
      },
    });
  }

  private resetEditor() {
    this.edit = false;
    this.editingUnsavedResource = false;
    this.resource.next(undefined);
    // ToDo: Navigating in a service doesn't follow the seperation of concern principle
    this.router.navigate(['']);
  }

  private async showErrorSnackbar(error: HttpErrorResponse) {
    if (error.status === 0) {
      // Unknown errors occur most commonly when the server is not reachable.
      // That is handled somewhere else, so there is no need to show that here.
      return;
    }

    let translation: string;
    if (error.status === HttpStatusCode.NotFound) {
      translation = await this.getTranslation(TranslationConstants.DEFAULT_VIEW.NOT_FOUND);
    } else {
      translation = (await this.getTranslation(TranslationConstants.DEFAULT_VIEW.FAILED_LOADING)) + ` ${error.status}`;
    }
    await this.snackbarService.showError(translation);
  }

  private async getTranslation(key: string) {
    const translations = await this.translate.get([key]).toAsync();
    return translations[key];
  }

  // ToDo: Call in on destroy
  stashResource() {
    if (!this.resource.value) return;

    this.sessionService.setWipResource(this.resource.value, <ResourceStorageDetails>{
      createNewResource: this.editingUnsavedResource,
    });
  }

  loadFromStorage() {
    const resourceStorageObject = this.sessionService.getWipResource();
    if (!resourceStorageObject) return;
    this.editingUnsavedResource = resourceStorageObject.details.createNewResource;
    this.resource.next(resourceStorageObject.resource);
  }

  async addNewResource(parameters: ResourceConstructionParameters, parent: ResourceModel | undefined): Promise<ResourceModel | undefined> {
    const resource = await this.modificationService
      .constructWithParameters({
        type: parameters.name,
        method: parameters?.method?.name ?? undefined,
        body: parameters.method?.parameters,
      })
      .toAsync()
      .catch(async (e: HttpErrorResponse) => await this.showErrorSnackbar(e));
    if (!resource) return;

    this.editingUnsavedResource = resource.id === 0;

    // When the resource was already save, other resources might also be
    if (!this.editingUnsavedResource) await this.cacheService.loadResources();

    if (parent) this.assignReferences(resource, parent);

    this.resource.next(resource);
    this.edit = true;
    return resource;
    //this.navigateToResource(resource);
  }


  removeResource() {
    this.resetEditor();
  }

  onEdit() {
    this.edit = true;
  }

  async onSave(): Promise<ResourceModel | undefined> {
    const resourceModel = this.resource.getValue();
    if (!resourceModel) return;

    if (resourceModel.properties) PrototypeToEntryConverter.convertToEntry(resourceModel.properties);

    if (this.editingUnsavedResource)
      return await this.modificationService
        .save$Response({ body: resourceModel })
        .toAsync()
        .then(async response => await this.handleSaveResponse(response))
        .catch(async error => {
          await this.showErrorSnackbar(error);
          return undefined;
        });
    else
      return await this.modificationService
        .update$Response({ id: resourceModel.id!, body: resourceModel })
        .toAsync()
        .then(async response => await this.handleUpdateResponse(response))
        .catch(async (e: HttpErrorResponse) => {
          await this.showErrorSnackbar(e);
          return undefined;
        });
  }

  async handleUpdateResponse(response: StrictHttpResponse<ResourceModel>): Promise<ResourceModel> {
    await this.cacheService.loadResources();
    this.resource.next(response.body);
    this.edit = false;
    return response.body;
  }

  async handleSaveResponse(response: StrictHttpResponse<ResourceModel>): Promise<ResourceModel> {
    // load all resources in order to also find resources, which were created automatically in the backend
    // ToDo: Handing over the event through both services seems suboptimal, violates the SR principle for this method.
    await this.cacheService.loadResources();
    const resourceModel = response.body;
    this.editingUnsavedResource = false;
    this.edit = false;

    this.resource.next(resourceModel);
    return resourceModel;
  }

  onCancel() {
    this.edit = false;
    this.loadResource();
  }

  onDeselect() {
    this.resetEditor();
  }

  assignReferences(resource: ResourceModel, parent: ResourceModel) {
    const referenceToParent = resource.references?.find(r => r.name == 'Parent');
    if (referenceToParent) referenceToParent.targets = [parent] as ResourceModel[];
    else
      resource.references?.push({
        name: 'Parent',
        targets: [parent] as ResourceModel[],
      } as ResourceReferenceModel);
  }
}

