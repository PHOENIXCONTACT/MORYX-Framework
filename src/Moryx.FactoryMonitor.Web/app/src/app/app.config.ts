/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, inject, provideAppInitializer } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { provideRouter } from '@angular/router';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

import { environment } from '../environments/environment';
import { FactoryMonitorService } from './api/services';
import { routes } from './app.routes';
import { CellStoreService } from './services/cell-store.service';
import { FactorySelectionService } from './services/factory-selection.service';
import { OrderStoreService } from './services/order-store.service';
import { provideApiConfiguration } from '@api/api-configuration';
import { API_INTERCEPTOR_PROVIDER, ApiInterceptor } from '@moryx/ngx-web-framework/interceptors';

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
    provideAppInitializer(() => {
      // Use material-symbols as default icon font
      const iconRegistry = inject(MatIconRegistry);
      iconRegistry.setDefaultFontSetClass('material-symbols-outlined');
    }),
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
    }),
  ]
};
