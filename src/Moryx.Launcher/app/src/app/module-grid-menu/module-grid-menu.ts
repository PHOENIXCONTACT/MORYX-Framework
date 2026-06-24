/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ViewChild } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenu, MatMenuModule } from '@angular/material/menu';
import { ModuleItem } from '../models/module-item';
import { ModuleService } from '../services/module.service';
import { LocationPersistenceService } from '../services/location-persistence.service';

@Component({
  selector: 'app-module-grid-menu',
  imports: [MatIconModule, MatMenuModule],
  templateUrl: './module-grid-menu.html',
  styleUrl: './module-grid-menu.scss'
})
export class ModuleGridMenu {
  private moduleService = inject(ModuleService);
  private locationPersistence = inject(LocationPersistenceService);

  modules = this.moduleService.userModules;
  activeRoute = this.moduleService.activeRoute;

  @ViewChild('moduleGridMenu') moduleGridMenu!: MatMenu;

  resolveHref(module: ModuleItem): string {
    return this.locationPersistence.resolveHref(module);
  }
}
