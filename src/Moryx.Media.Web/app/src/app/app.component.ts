/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit } from '@angular/core';
import { LanguageService } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { MediaService } from './services/media-service/media.service';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [RouterOutlet]
})
export class AppComponent implements OnInit {
  title = 'Moryx.Media.Web';

  TranslationConstants = TranslationConstants;

  constructor(
    private mediaservice: MediaService,
    private languageService: LanguageService,
    public translate: TranslateService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.mediaservice.loadContents();
  }
}

