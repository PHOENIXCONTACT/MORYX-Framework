/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { SessionService } from './services/session.service';

/**
 * Verifies if there is a work in progress product in the session storage and redirects
 * to the details view of this product to continue work, if there is one.
 *
 * @returns true if there is no work in progress product, a RedirectCommand otherwise.
 */
export const WorkInProgressGuard: CanActivateFn = () => {
  const sessionService = inject(SessionService);
  const router = inject(Router);

  const workInProgress = sessionService.getWipProduct();
  if (workInProgress) {
    return new RedirectCommand(router.parseUrl(`/details/${workInProgress.product.id}`));
  }

  return true;
};
