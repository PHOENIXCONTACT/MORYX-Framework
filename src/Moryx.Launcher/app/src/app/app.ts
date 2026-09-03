/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, DestroyRef, effect, inject, input, untracked, ViewEncapsulation } from '@angular/core';
import { WebModuleItem } from './models/web-module-item';
import { FullLayout } from './layouts/full-layout/full-layout';
import { LauncherLayout, LauncherLayoutService } from './services/launcher-layout.service';
import { ExternalModuleItem } from './models/external-module-item';
import { CultureModel } from './models/culture-model';
import { OperatorLayout } from './layouts/operator-layout/operator-layout';
import { FullscreenLayout } from './layouts/fullscreen-layout/fullscreen-layout';
import { ModuleService } from './services/module.service';
import { CultureService } from './services/culture.service';
import { AuthService } from './services/auth.service';
import { ShortcutService } from './services/shortcut.service';
import { TranslationConstants } from './translation-constants';

import { SpotlightSearch } from './spotlight-search/spotlight-search';
@Component({
  selector: 'app-root',
  imports: [FullLayout, OperatorLayout, FullscreenLayout, SpotlightSearch],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class App {

  private launcherLayoutService = inject(LauncherLayoutService);
  private moduleService = inject(ModuleService);
  private cultureService = inject(CultureService);
  private authService = inject(AuthService);
  private shortcutService = inject(ShortcutService);
  private destroyRef = inject(DestroyRef);

  // Web component inputs
  readonly webModuleItems = input.required<WebModuleItem[]>();
  readonly externalModuleItems = input.required<ExternalModuleItem[]>();
  readonly supportedCultures = input.required<CultureModel[]>();
  readonly authBaseAddress = input<string>();

  protected layout = this.launcherLayoutService.layout;

  constructor() {
    this.shortcutService.register(
      {
        key: 'Digit1',
        ctrl: true,
        alt: true,
        label: TranslationConstants.SHORTCUTS.FULL_MODE,
        action: () => this.launcherLayoutService.updateLayout(LauncherLayout.Full) },
      this.destroyRef
    );
    this.shortcutService.register(
      {
        key: 'Digit2',
        ctrl: true,
        alt: true,
        label: TranslationConstants.SHORTCUTS.OPERATOR_MODE,
        action: () => this.launcherLayoutService.updateLayout(LauncherLayout.Operator)
      },
      this.destroyRef
    );
    this.shortcutService.register(
      {
        key: 'Digit3',
        ctrl: true, alt: true,
        label: TranslationConstants.SHORTCUTS.FULLSCREEN_MODE,
        action: () => this.launcherLayoutService.updateLayout(LauncherLayout.Fullscreen)
      },
      this.destroyRef
    );

    effect(() => {
      const modules = [...this.webModuleItems(), ...this.externalModuleItems()];
      untracked(() => {
        this.moduleService.setModules(modules);
      });
    });

    effect(() => {
      const cultures = this.supportedCultures();
      untracked(() => {
        this.cultureService.setSupportedCultures(cultures);
      });
    });

    effect(() => {
      const authBaseAddress = this.authBaseAddress();
      untracked(() => {
        this.authService.setAuthBaseAddress(authBaseAddress);
      });
    });

  }
}

