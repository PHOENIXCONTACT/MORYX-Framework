/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { EditResourceService } from 'src/app/services/edit-resource.service';

/**
 * Resets the edit service when navigating to the app, e.g. 
 * on errors when failing API calls or when navigating back
 * to the app after canceling the creation of an unsaved resource.
 */
export const AppResolver: ResolveFn<void> = async () => {
  const editService = inject(EditResourceService);
  editService.resetEditor();
};
