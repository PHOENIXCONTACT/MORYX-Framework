/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, HostListener, inject, signal } from '@angular/core';
import { LauncherLayout, LauncherStateService } from '../services/launcher-state.service';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
})
export class FullscreenLayout {

  private launcherStateService = inject(LauncherStateService);

  hasRightRegion = signal(false);

  @HostListener('window:keydown.escape')
  exitFullscreen() {
    this.launcherStateService.updateLayout(LauncherLayout.Full);
  }

  onRightRegionSlotChange(event: Event): void {
    const slot = event.target as HTMLSlotElement;
    this.hasRightRegion.set(slot.assignedNodes().length > 0);
  }
}
