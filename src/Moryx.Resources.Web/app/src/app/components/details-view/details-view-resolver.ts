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
 * Retrieves the product details given the product id from the route before navigating to the details view.
 * If an error occurs during retrieval, it handles the error and redirects to the default view.
 *
 * This ought to be the only place where the product details are retrieved from the API.
 * The retrieved product is stored in the EditProductsService and can be accessed by all child components of the details view.
 */
export const DetailsViewResolver: ResolveFn<ResourceModel> = async (route: ActivatedRouteSnapshot) => {
  const apiService = inject(ResourceModificationService);
  const sessionService = inject(SessionService);
  const editService = inject(EditResourceService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);
  const id = Number(route.paramMap.get('id'));

  // If there is a product that was work in progress and we are not navigating to a
  // different product, use the product from the session storage instead of retrieving it again from the API.
  const workInProgress = sessionService.popWipResource();
  if (workInProgress?.resource.id === id) {
    editService.setResourceFromStorage(workInProgress);
    return workInProgress.resource;
  }

  try {    
    const resource = await lastValueFrom(apiService.getDetails({id: id}));
    editService.setResource(resource);
    return resource;
  }
  catch (error) {
    await snackbarService.handleError(error as HttpErrorResponse);
    editService.resetEditor();
    return new RedirectCommand(router.parseUrl(''));
  }
};

