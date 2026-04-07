/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RedirectCommand, ResolveFn, Router } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionService } from 'src/app/services/session.service';
import { ResourceModel } from 'src/app/api/models';
import { EditResourceService } from 'src/app/services/edit-resource.service';
import { ResourceModificationService } from 'src/app/api/services';

/**
 * Retrieves the resource details given the resource id from the route before navigating to the details view.
 * If an error occurs during retrieval, it handles the error and redirects to the default view.
 *
 * This ought to be the only place where the resource details are retrieved from the API.
 * The retrieved resource is stored in the EditResourceService and can be accessed by all child components of the details view.
 */
export const DetailsViewResolver: ResolveFn<ResourceModel> = async (route: ActivatedRouteSnapshot) => {
  const apiService = inject(ResourceModificationService);
  const sessionService = inject(SessionService);
  const editService = inject(EditResourceService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);
  const id = Number(route.paramMap.get('id'));

  // If there is a resource that was work in progress and we are not 
  // navigating to a different resource, use the resource from the session 
  // storage instead of retrieving it again from the API.
  const workInProgress = sessionService.removeWipResource();
  if (workInProgress?.resource.id === id) {
    editService.setResourceFromStorage(workInProgress);
    return workInProgress.resource;
  }
 
  // If the ID is still 0, we should be currently creating a resource and the 
  // edit service already holds the resource with all changes
  if (id === 0) {
    const resource = editService.activeResource();
    if (resource)
      return resource;
    else
      return new RedirectCommand(router.parseUrl('')); 
  }

  // Otherwise, we need to retrieve the resource details from the API
  try {    
    const resource = await lastValueFrom(apiService.getDetails({id: id}));
    editService.setResource(resource);
    return resource;
  }
  catch (error) {
    await snackbarService.handleError(error as HttpErrorResponse);
    return new RedirectCommand(router.parseUrl(''));
  }
};
