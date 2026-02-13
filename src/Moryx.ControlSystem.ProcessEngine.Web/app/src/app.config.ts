/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, enableProdMode, importProvidersFrom } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatDividerModule } from "@angular/material/divider";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from "@moryx/ngx-web-framework";
import { ApiModule } from "./app/api/api.module";
import { environment } from "./environments/environment";
import { routes } from "./app.routes";
import { provideRouter } from "@angular/router";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

if (environment.production) {
  enableProdMode();
}


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      ApiModule.forRoot({rootUrl: environment.rootUrl}),
      MatExpansionModule,
      MatListModule,
      MatCardModule,
      MatGridListModule,
      MatProgressBarModule,
      MatProgressSpinnerModule,
      MatButtonModule,
      MatIconModule,
      MatTableModule,
      MatDividerModule,
      MatSlideToggleModule,
      MatSnackBarModule
    ),
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    provideHttpClient(withInterceptorsFromDi()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),
    provideAnimations(),
  ],
};

