/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { RedirectCommand, ResolveFn, Router } from '@angular/router';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { ProductManagementService } from 'src/app/api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { EditProductsService } from 'src/app/services/edit-products.service';
import { ProductModel, ProductQuery, RevisionFilter, Selector } from 'src/app/api/models';

/**
 * Retrieves the product details given the product id from the route before navigating to the details view.
 * If an error occurs during retrieval, it handles the error and redirects to the default view.
 *
 * This ought to be the only place where the product details are retrieved from the API.
 * The retrieved product is stored in the EditProductsService and can be accessed by all child components of the details view.
 */
export const ProductReferencesResolver: ResolveFn<ProductModel[]> = async () => {
  const apiService = inject(ProductManagementService);
  const editService = inject(EditProductsService);
  const snackbarService = inject(SnackbarService);
  const router = inject(Router);

  const product = await firstValueFrom(editService.currentProduct$);
  if (!product) {
    throw new Error('Invalid State: Tried to resolve product references without a current product');
  }

  const body = <ProductQuery>{
    includeDeleted: false,
    identifier: product.identifier,
    revision: product.revision,
    revisionFilter: RevisionFilter.Specific,
    selector: Selector.Parent,
  };

  try {
    const references = await lastValueFrom(apiService.getTypes({body: body}));
    editService.setReferences(references);
    return references;
  }
  catch (error) {
    await snackbarService.handleError(error as HttpErrorResponse);
    return new RedirectCommand(router.parseUrl(`/details/${product.id}`));
  }
};

