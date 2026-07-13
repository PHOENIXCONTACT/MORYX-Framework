/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ApplicationConfig, provideEnvironmentInitializer } from "@angular/core";
import { MatIconRegistry } from '@angular/material/icon';
import { provideAppInitializer, inject } from '@angular/core';
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from '@moryx/ngx-web-framework/interceptors';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { environment } from "../environments/environment";
import { CacheResourceService } from "./services/cache-resource.service";
import { SearchService } from "./services/search.service";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideApiConfiguration } from '@api/api-configuration';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    // Configure the API endpoint
    provideApiConfiguration(environment.rootUrl),

    // Register custom DI interceptors
    // TODO: Replace by fns, if https://github.com/PHOENIXCONTACT/ngx-moryx-web/pull/48 was released
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,

    // Setup HttpClient
    // TODO: Remove withInterceptorsFromDi if https://github.com/PHOENIXCONTACT/ngx-moryx-web/pull/48 was released
    provideHttpClient(withInterceptorsFromDi()),

    // Configure translation loader
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),

    // Additional app initializers
    provideEnvironmentInitializer(() => inject(SearchService)),
    provideAppInitializer(() => {
      const initializerFn = (
        (resourceCache: CacheResourceService) => async () =>
          await resourceCache.loadResources()
      )(inject(CacheResourceService));
      return initializerFn();
    }),
    provideAppInitializer(() => {
      // Use material-symbols as default icon font
      const iconRegistry = inject(MatIconRegistry);
      iconRegistry.setDefaultFontSetClass('material-symbols-outlined');
    }),
  ]
}
