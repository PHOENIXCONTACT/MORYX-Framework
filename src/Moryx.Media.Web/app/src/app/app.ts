/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit } from '@angular/core';
import { LanguageService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { MediaService } from './services/media-service/media.service';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrls: ['./app.scss'],
    imports: [RouterOutlet]
})
export class App implements OnInit {
  private mediaService = inject(MediaService);
  private languageService = inject(LanguageService);
  private translateService = inject(TranslateService);

  title = 'Moryx.Media.Web';

  TranslationConstants = TranslationConstants;

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang('en');
    this.translateService.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.mediaService.loadContents();
  }
}

