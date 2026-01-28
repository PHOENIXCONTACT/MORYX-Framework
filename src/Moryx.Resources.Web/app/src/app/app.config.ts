/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ApplicationConfig } from "@angular/core";
import { provideAnimations } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTreeModule } from '@angular/material/tree';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatStepperModule } from '@angular/material/stepper';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTooltipModule } from '@angular/material/tooltip';
import { provideAppInitializer, inject, importProvidersFrom } from '@angular/core';
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from '@moryx/ngx-web-framework/interceptors';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { provideHttpClient, withInterceptorsFromDi, HttpClient } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { BrowserModule } from "@angular/platform-browser";
import { ApiModule } from "./api/api.module";
import { CacheResourceService } from "./services/cache-resource.service";
import { FormControlService } from "./services/form-control-service.service";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      FormsModule,
      ReactiveFormsModule,
      MatTreeModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule,
      MatCardModule,
      MatListModule,
      MatTabsModule,
      MatIconModule,
      MatTableModule,
      MatMenuModule,
      MatProgressSpinnerModule,
      MatDialogModule,
      MatSelectModule,
      MatSnackBarModule,
      MatSidenavModule,
      MatStepperModule,
      MatToolbarModule,
      MatExpansionModule,
      ApiModule.forRoot({rootUrl: environment.rootUrl}),
      MatTooltipModule
    ),
    provideAppInitializer(() => {
      const initializerFn = (
        (resourceCache: CacheResourceService) => async () =>
          await resourceCache.loadResources()
      )(inject(CacheResourceService));
      return initializerFn();
    }),
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    SnackbarService,
    FormControlService,
    provideHttpClient(withInterceptorsFromDi()),
    provideHttpClient(withInterceptorsFromDi()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),
    provideAnimations(),
  ]
}
