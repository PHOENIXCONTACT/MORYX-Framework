/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { DragDropModule } from '@angular/cdk/drag-drop';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, importProvidersFrom, inject, provideAppInitializer } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule } from '@angular/platform-browser';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiModule } from '@api/api.module';
import { FactoryMonitorService } from './api/services';
import { routes } from './app.routes';
import { CellSettingsService } from './services/cell-settings.service';
import { CellStoreService } from './services/cell-store.service';
import { ChangeBackgroundService } from './services/change-background.service';
import { EditMenuService } from './services/edit-menu.service';
import { FactorySelectionService } from './services/factory-selection.service';
import { OrderStoreService } from './services/order-store.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      BrowserModule,
      MatIconModule,
      MatButtonModule,
      MatListModule,
      MatDialogModule,
      MatInputModule,
      MatTooltipModule,
      DragDropModule,
      FormsModule,
      ReactiveFormsModule,
      MatSnackBarModule
    ),
    OrderStoreService,
    CellStoreService,
    EditMenuService,
    ChangeBackgroundService,
    CellSettingsService,
    provideHttpClient(withInterceptorsFromDi()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),
    provideAnimationsAsync(),
    provideAppInitializer(async () => {
      const api = inject(FactoryMonitorService);
      const orderStore = inject(OrderStoreService);
      const cellStore = inject(CellStoreService);
      const factorySelectionService = inject(FactorySelectionService);

      // ToDo: Error Handling
      const initialState = await firstValueFrom(api.initialFactoryState());

      orderStore.initialize(initialState);

      cellStore.initialize(initialState);

      factorySelectionService.initialize(initialState);
    }),
  ]
};
