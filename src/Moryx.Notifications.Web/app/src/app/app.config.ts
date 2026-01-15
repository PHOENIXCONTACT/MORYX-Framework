/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  HttpClient,
  provideHttpClient,
  withInterceptorsFromDi,
} from "@angular/common/http";
import { provideRouter } from "@angular/router";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { environment } from "src/environments/environment";
import { routes } from "./app.routes";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatToolbarModule } from "@angular/material/toolbar";
import { provideAnimations } from "@angular/platform-browser/animations";
import {
  ApiInterceptor,
  API_INTERCEPTOR_PROVIDER,
  MoryxSnackbarService,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { ApiModule } from "./api/api.module";
import { MarkdownModule } from "ngx-markdown";

function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + "assets/languages/"
  );
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom([
      MarkdownModule.forRoot(),
      BrowserModule,
      MatProgressSpinnerModule,
      MatIconModule,
      MatButtonModule,
      MatCardModule,
      MatToolbarModule,
      MatSidenavModule,
      MatSnackBarModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: httpTranslateLoaderFactory,
          deps: [HttpClient],
        },
      }),
    ]),
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    MoryxSnackbarService,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};

