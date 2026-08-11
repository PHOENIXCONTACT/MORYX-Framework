/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { ApplicationConfig, enableProdMode } from "@angular/core";
import { provideMoryxMaterialDefaults } from "@moryx/ngx-web-framework/material";
import { languageInterceptor, apiErrorInterceptor } from "@moryx/ngx-web-framework/interceptors";
import { environment } from "../environments/environment";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideApiConfiguration } from '@api/api-configuration';

if (environment.production) {
  enableProdMode();
}

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
      fallbackLang: 'en'
    }),

    // Provides angular material defaults
    provideMoryxMaterialDefaults()
  ],
};

