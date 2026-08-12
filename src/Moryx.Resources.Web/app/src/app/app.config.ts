/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ApplicationConfig, provideEnvironmentInitializer } from "@angular/core";
import { provideAppInitializer, inject } from '@angular/core';
import { provideMoryxMaterialDefaults } from '@moryx/ngx-web-framework/material';
import { provideMoryxLocalization } from '@moryx/ngx-web-framework/i18n';
import { TranslationConstants } from './translation-constants';
import { languageInterceptor, apiErrorInterceptor } from '@moryx/ngx-web-framework/interceptors';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { environment } from "../environments/environment";
import { CacheResourceService } from "./services/cache-resource.service";
import { SearchService } from "./services/search.service";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideApiConfiguration } from '@api/api-configuration';

// Register locale data for built-in Angular pipes (date, number, etc.)
import '@angular/common/locales/global/de';
import '@angular/common/locales/global/it';
import '@angular/common/locales/global/zh';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    // Configure the API endpoint
    provideApiConfiguration(environment.rootUrl),

    // Setup HttpClient with functional interceptors
    provideHttpClient(
      withInterceptors([languageInterceptor, apiErrorInterceptor])
    ),

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
    provideEnvironmentInitializer(() => inject(SearchService)),
    provideAppInitializer(async () => {
      const cacheResourceService = inject(CacheResourceService)
      await cacheResourceService.loadResources()
    }),
  ]
}
