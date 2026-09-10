/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ApplicationConfig, inject, provideAppInitializer } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { provideMoryxLocalization } from '@moryx/ngx-web-framework/i18n';
import { languageInterceptor, apiErrorInterceptor } from '@moryx/ngx-web-framework/interceptors';
import { provideMoryxMaterialDefaults } from '@moryx/ngx-web-framework/material';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

import { provideApiConfiguration } from '@api/api-configuration';
import { FactoryMonitorService } from './api/services';
import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { CellStoreService } from './services/cell-store.service';
import { FactorySelectionService } from './services/factory-selection.service';
import { OrderStoreService } from './services/order-store.service';
import { TranslationConstants } from './translation-constants';

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
    provideAppInitializer(async () => {
      const api = inject(FactoryMonitorService);
      const orderStore = inject(OrderStoreService);
      const cellStore = inject(CellStoreService);
      const factorySelectionService = inject(FactorySelectionService);

      // ToDo: Error Handling
      const initialState = await api.initialFactoryState();

      orderStore.initialize(initialState);

      cellStore.initialize(initialState);

      factorySelectionService.initialize(initialState);
    })
  ]
};
