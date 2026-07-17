/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideMoryxMaterialDefaults } from '@moryx/ngx-web-framework/material';

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { environment } from '../environments/environment';

import { LocationPersistenceService } from './services/location-persistence.service';
import { provideApiConfiguration } from '@api/api-configuration';

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
      fallbackLang: 'en'
    }),

    // Provides angular material defaults
    provideMoryxMaterialDefaults(),

    // Additional app initializers
    provideAppInitializer(() => {
      // Ensure instantiation of LocationPersistenceService to register the location change listener
      inject(LocationPersistenceService);
    })
  ]
};
