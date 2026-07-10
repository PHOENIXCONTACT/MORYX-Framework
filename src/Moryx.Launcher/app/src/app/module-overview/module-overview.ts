/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, input } from '@angular/core';
import { ModuleCategory } from '../models/module-category';
import { ModuleItem } from '../models/module-item';
import { WebModuleItem } from '../models/web-module-item';
import { LocationPersistenceService } from '../services/location-persistence.service';

@Component({
  selector: 'app-module-overview',
  imports: [],
  templateUrl: './module-overview.html',
  styleUrl: './module-overview.scss',
})
export class ModuleOverview {
  private locationPersistenceService = inject(LocationPersistenceService);

  readonly webModuleItems = input.required<WebModuleItem[]>();
  protected userModules = computed(() => this.webModuleItems().filter(m => m.category === ModuleCategory.User));

  protected resolveHref(module: ModuleItem): string {
    return this.locationPersistenceService.resolveHref(module);
  }
}
