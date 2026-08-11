/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { WebModuleItem } from '../models/web-module-item';
import { ExternalModuleItem } from '../models/external-module-item';
import { ModuleItem } from '../models/module-item';
import { ModuleService } from '../services/module.service';
import { LocationPersistenceService } from '../services/location-persistence.service';
import { TranslationConstants } from '../translation-constants';

export abstract class ModuleNavBase {
  private moduleService = inject(ModuleService);
  private locationPersistenceService = inject(LocationPersistenceService);

  protected modules = this.moduleService.userModules;
  protected activeRoute = this.moduleService.activeRoute;

  protected TranslationConstants = TranslationConstants;

  protected resolveHref(module: ModuleItem): string {
    return this.locationPersistenceService.resolveHref(module);
  }

  protected asWeb(item: WebModuleItem | ExternalModuleItem): WebModuleItem | null {
    return 'eventStream' in item ? item as WebModuleItem : null;
  }
}
