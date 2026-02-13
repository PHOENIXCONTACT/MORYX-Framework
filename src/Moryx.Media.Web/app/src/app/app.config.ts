/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ClipboardModule } from "@angular/cdk/clipboard";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { MatBadgeModule } from "@angular/material/badge";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { provideRouter } from "@angular/router";
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from "@moryx/ngx-web-framework/interceptors";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      MatBadgeModule,
      MatCardModule,
      MatButtonModule,
      MatGridListModule,
      MatProgressSpinnerModule,
      MatDialogModule,
      MatListModule,
      MatIconModule,
      MatTooltipModule,
      MatFormFieldModule,
      MatInputModule,
      FormsModule,
      MatSnackBarModule,
      MatMenuModule,
      MatSidenavModule,
      MatToolbarModule,
      NgxDocViewerModule,
      ClipboardModule,
      ApiModule.forRoot({rootUrl: environment.rootUrl})
    ),
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    SnackbarService,
    provideHttpClient(withInterceptorsFromDi()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),
    provideAnimations()
  ]
}
