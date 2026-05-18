/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RedirectCommand, ResolveFn, Router } from '@angular/router';
import { PartModel } from 'src/app/api/models';
import { EditProductsService } from 'src/app/services/edit-products.service';
import { toSignal } from '@angular/core/rxjs-interop';

/**
 * Sets the current part connector and part in the EditProductsService based on the partName and partId route parameter. 
 * If the either of them is not found, it redirects to the base connector or the first index of the connector page.
 */
export const PartsResolver: ResolveFn<PartModel | undefined> = async (route: ActivatedRouteSnapshot) => {
  const editService = inject(EditProductsService);
  const router = inject(Router);
  const currentProduct = toSignal(editService.currentProduct$)();
  const partId = Number(route.paramMap.get('partId'));
  const connectorName = route.paramMap.get('partName');

  if (!currentProduct) {
    throw new Error('Invalid State: Tried to resolve product parts without a current product');
  }

  const connector = currentProduct.parts?.find(p => p.name === connectorName);
  editService.setPartConnector(connector);
  // Base path for the parts view, no connector of part selected
  if (connectorName === 'base') {
    return undefined;
  }
  // Connector not found, redirect to base path of parts view
  if (!connector) {
    return new RedirectCommand(router.createUrlTree(['details', currentProduct.id, 'parts', 'base', 0]));
  }
  const part = connector.parts?.find(p => p.id === partId);
  editService.setPart(part);
  // No part id provided, i.e. stay on base path of the connector
  if (!partId) {
    return undefined;
  }
  // Part not found, redirect to first part of the connector if it exists, otherwise redirect to base path of the connector
  if (!part) {
    const firstPartId = connector.parts && connector.parts.length > 0 ? connector.parts[0].id : 0;
    return new RedirectCommand(router.createUrlTree(['details', currentProduct.id, 'parts', connectorName, firstPartId]));
  }
  // Connector and part found, set them in the service and return the part for the resolver
  return part;
};
