/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RedirectCommand, ResolveFn, Router } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { EditProductsService } from '../../services/edit-products.service';
import { ProductModel } from '../../api/models';
import { ProductManagementService } from 'src/app/api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionService } from 'src/app/services/session.service';

/**
 * Retrieves the product details given the product id from the route before navigating to the details view.
 * If an error occurs during retrieval, it handles the error and redirects to the default view.
 *
 * This ought to be the only place where the product details are retrieved from the API.
 * The retrieved product is stored in the EditProductsService and can be accessed by all child components of the details view.
 */
export const ProductResolver: ResolveFn<ProductModel> = async (route: ActivatedRouteSnapshot) => {
  const apiService = inject(ProductManagementService);
  const sessionService = inject(SessionService);
  const editService = inject(EditProductsService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);
  const id = Number(route.paramMap.get('id'));

  // If there is a product that was work in progress and we are not navigating to a
  // different product, use the product from the session storage instead of retrieving it again from the API.
  const workInProgress = sessionService.popWipProduct();
  if (workInProgress?.product.id === id) {
    editService.setProductFromStorage(workInProgress);
    return workInProgress.product;
  }

  try {
    const product = await lastValueFrom(apiService.getTypeById({ id: id }));
    editService.setProduct(product);
    return product;
  }
  catch (error) {
    await snackbarService.handleError(error as HttpErrorResponse);
    editService.resetProduct();
    return new RedirectCommand(router.parseUrl(''));
  }
};

