/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, input, ViewEncapsulation } from '@angular/core';
import { WebModuleItem } from './models/web-module-item';
import { FullLayout } from './full-layout/full-layout';
import { LauncherStateService } from './services/launcher-state.service';
import { ExternalModuleItem } from './models/external-module-item';
import { CultureModel } from './models/culture-model';
import { OperatorLayout } from './operator-layout/operator-layout';
import { FullscreenLayout } from './fullscreen-layout/fullscreen-layout';
import { ModuleService } from './services/module.service';
import { CultureService } from './services/culture.service';
import { AuthService } from './services/auth.service';
import { LanguageService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './translation-constants';
import { MoryxLauncherShell } from './moryx-launcher-shell';
import { SpotlightSearch } from './spotlight-search/spotlight-search';
import { SearchService } from './services/search.service';

@Component({
  selector: 'app-root',
  imports: [FullLayout, OperatorLayout, FullscreenLayout, SpotlightSearch],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class App {

  // Web component inputs
  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();
  authBaseAddress = input<string>();

  private launcherStateService = inject(LauncherStateService);
  private moduleService = inject(ModuleService);
  private cultureService = inject(CultureService);
  private authService = inject(AuthService);
  private languageService = inject(LanguageService);
  private translateService = inject(TranslateService);
  private searchService = inject(SearchService);

  layout = this.launcherStateService.layout;

  constructor() {
    window.shell = new MoryxLauncherShell(this.cultureService, this.searchService);

    effect(() => {
      this.moduleService.modules.set([...this.webModuleItems(), ...this.externalModuleItems()]);
      this.cultureService.supportedCultures.set(this.supportedCultures());

      const authBaseAddress = this.authBaseAddress();
      this.authService.authBaseAddress = authBaseAddress;
      this.authService.authConfigured.set(!!authBaseAddress && authBaseAddress.length > 0);
    });

    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translateService.setFallbackLang("en");

    console.log("Language: " + this.languageService.getFallbackLang());
    this.translateService.use(this.languageService.getFallbackLang());
  }
}

