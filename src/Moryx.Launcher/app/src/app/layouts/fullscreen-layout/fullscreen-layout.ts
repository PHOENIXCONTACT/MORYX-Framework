/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { LauncherLayout } from '../../services/launcher-state.service';
import { LayoutBase } from '../layout-base';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
  host: {
    '(window:keydown)': 'onKeyDown($event)',
  }
})
export class FullscreenLayout extends LayoutBase {

  onKeyDown(event: KeyboardEvent) {
    if (event.key === 'Escape' && !event.defaultPrevented) {
      this.exitFullscreen();
    }
  }

  exitFullscreen() {
    this.launcherStateService.updateLayout(LauncherLayout.Full);
  }
}
