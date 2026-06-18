/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject } from '@angular/core';
import { LauncherStateService } from '../services/launcher-state.service';

export abstract class LayoutBase {
  protected launcherStateService = inject(LauncherStateService);

  showTopRegion = computed(() =>
    this.launcherStateService.topRegionAvailable() && this.launcherStateService.topRegionEnabled()
  );

  showRightRegion = computed(() =>
    this.launcherStateService.rightRegionAvailable() && this.launcherStateService.rightRegionEnabled()
  );

  onTopRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.launcherStateService.topRegionAvailable.set(slot.assignedNodes().length > 0);
  }

  onRightRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.launcherStateService.rightRegionAvailable.set(slot.assignedNodes().length > 0);
  }
}
