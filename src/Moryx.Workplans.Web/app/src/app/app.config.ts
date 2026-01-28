/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { DragDropModule } from '@angular/cdk/drag-drop';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from '@moryx/ngx-web-framework';
import { environment } from 'src/environments/environment';
import { ApiModule } from './api/api.module';
import { BrowserStorageService } from './services/browser-storage.service';
import { EditorStateService } from './services/editor-state.service';
import { SessionsService } from './services/sessions.service';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      DragDropModule,
      HammerModule,
      MatListModule,
      MatButtonModule,
      MatTabsModule,
      MatCardModule,
      MatIconModule,
      MatExpansionModule,
      MatDialogModule,
      MatTooltipModule,
      MatSnackBarModule,
      MatToolbarModule,
      MatSidenavModule,
      FormsModule,
      MatInputModule,
      MatProgressSpinnerModule,
      MatFormFieldModule,
      MatSelectModule,
      MatMenuModule,
      MatTableModule
    ),
    BrowserStorageService,
    SessionsService,
    EditorStateService,
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

