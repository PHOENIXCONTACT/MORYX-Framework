/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom, inject, provideAppInitializer } from "@angular/core";
import { MAT_DIALOG_DEFAULT_OPTIONS } from "@angular/material/dialog";
import { MatIconRegistry } from "@angular/material/icon";
import { BrowserModule } from "@angular/platform-browser";
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from "@moryx/ngx-web-framework/interceptors";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { environment } from "../environments/environment";
import { ApiModule } from "@api/api.module";
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideApiConfiguration } from '@api/api-configuration';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding(), withRouterConfig({paramsInheritanceStrategy: 'always'})),

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

    // Configure mat-dialog defaults
    {
      provide: MAT_DIALOG_DEFAULT_OPTIONS,
      useValue: {
        maxWidth: 'min(560px, 95vw)',
        maxHeight: '90vh'
      }
    },

    // Additional app initializers
    provideAppInitializer(() => {
      // Use material-symbols as default icon font
      const iconRegistry = inject(MatIconRegistry);
      iconRegistry.setDefaultFontSetClass('material-symbols-outlined');
    }),
  ],
};

