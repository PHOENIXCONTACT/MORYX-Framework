/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

import { provideMoryxLocalization } from '@moryx/ngx-web-framework/i18n';
import { provideMoryxMaterialDefaults } from '@moryx/ngx-web-framework/material';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

import { provideApiConfiguration } from '@api/api-configuration';
import { environment } from '../environments/environment';
import { LocationPersistenceService } from './services/location-persistence.service';
import { TranslationConstants } from './translation-constants';

// Register locale data for built-in Angular pipes (date, number, etc.)
import '@angular/common/locales/global/de';
import '@angular/common/locales/global/it';
import '@angular/common/locales/global/zh';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    // Configure the API endpoint
    provideApiConfiguration(environment.rootUrl),

    // Setup HttpClient
    provideHttpClient(),

    // Configure translation loader
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
    }),

    // Provides angular material defaults
    provideMoryxMaterialDefaults(),

    // Provides Angular locale and configures ngx-translate
    provideMoryxLocalization(TranslationConstants.LANGUAGES),

    // Additional app initializers
    provideAppInitializer(() => {
      // Ensure instantiation of LocationPersistenceService to register the location change listener
      inject(LocationPersistenceService);
    })
  ]
};
