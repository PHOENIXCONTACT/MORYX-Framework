/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LanguageService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  imports: [
    RouterOutlet
  ]
})
export class App implements OnInit {
  private translateService = inject(TranslateService);
  private languageService = inject(LanguageService);

  title = 'Orders';
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
    this.translateService.get([TranslationConstants.APP.TITLE]).subscribe(title => {
      this.title = title;
    });
  }
}

