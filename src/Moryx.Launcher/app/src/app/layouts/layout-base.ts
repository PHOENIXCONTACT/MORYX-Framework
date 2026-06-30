/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject } from '@angular/core';
import { LauncherLayoutService } from '../services/launcher-layout.service';

export abstract class LayoutBase {
  protected launcherLayoutService = inject(LauncherLayoutService);

  showTopRegion = computed(() =>
    this.launcherLayoutService.topRegionAvailable() && this.launcherLayoutService.topRegionEnabled()
  );

  showRightRegion = computed(() =>
    this.launcherLayoutService.rightRegionAvailable() && this.launcherLayoutService.rightRegionEnabled()
  );

  onTopRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.launcherLayoutService.topRegionAvailable.set(slot.assignedNodes().length > 0);
  }

  onRightRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.launcherLayoutService.rightRegionAvailable.set(slot.assignedNodes().length > 0);
  }
}
