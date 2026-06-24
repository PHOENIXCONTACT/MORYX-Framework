/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, input } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { WebModuleItem } from '../models/web-module-item';
import { NotificationBadge } from '../notification-badge/notification-badge';
import { ExternalModuleItem } from '../models/external-module-item';
import { ModuleItem } from '../models/module-item';
import { ModuleService } from '../services/module.service';
import { LocationPersistenceService } from '../services/location-persistence.service';

@Component({
  selector: 'app-vertical-module-nav',
  imports: [MatListModule, MatIconModule, NotificationBadge],
  templateUrl: './vertical-module-nav.html',
  styleUrl: './vertical-module-nav.scss',
  host: {
    '[class.collapsed]': 'collapsed()',
  }
})
export class VerticalModuleNav {
  private moduleService = inject(ModuleService);
  private locationPersistence = inject(LocationPersistenceService);

  modules = this.moduleService.userModules;
  activeRoute = this.moduleService.activeRoute;
  collapsed = input(false);

  resolveHref(module: ModuleItem): string {
    return this.locationPersistence.resolveHref(module);
  }

  asWeb(item: WebModuleItem | ExternalModuleItem): WebModuleItem | null {
    return 'eventStream' in item ? item as WebModuleItem : null;
  }
}
