/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, DestroyRef, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuTrigger } from '@angular/material/menu';
import { TranslatePipe } from '@ngx-translate/core';
import { ModuleGridMenu } from '../../navigation/module-grid-menu/module-grid-menu';
import { MoreMenu } from '../../more-menu/more-menu';
import { LauncherLayout } from '../../services/launcher-layout.service';
import { TranslationConstants } from '../../translation-constants';
import { LayoutBase } from '../layout-base';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [MatButtonModule, MatIconModule, MatMenuTrigger, ModuleGridMenu, MoreMenu, TranslatePipe],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
  host: {
    '(window:keydown)': 'onKeyDown($event)',
    '(window:mousemove)': 'onInteraction()',
    '(window:touchstart)': 'onInteraction()',
  }
})
export class FullscreenLayout extends LayoutBase {
  private destroyRef = inject(DestroyRef);

  protected TranslationConstants = TranslationConstants;
  private hideTimeout: ReturnType<typeof setTimeout> | null = null;

  protected showExitButton = signal(false);

  private static readonly HIDE_DELAY = 3000;

  constructor() {
    super();
    this.destroyRef.onDestroy(() => this.clearTimeout());
  }

  protected onKeyDown(event: KeyboardEvent) {
    this.onInteraction();
    if (event.key === 'Escape' && !event.defaultPrevented) {
      this.exitFullscreen();
    }
  }

  protected onInteraction() {
    this.showExitButton.set(true);
    this.clearTimeout();
    this.hideTimeout = setTimeout(() => this.showExitButton.set(false), FullscreenLayout.HIDE_DELAY);
  }

  protected exitFullscreen() {
    this.launcherLayoutService.updateLayout(LauncherLayout.Full);
  }

  private clearTimeout() {
    if (this.hideTimeout) {
      clearTimeout(this.hideTimeout);
      this.hideTimeout = null;
    }
  }
}
