/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogModule } from "@angular/material/dialog";
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { CellSettingsService } from "./services/cell-settings.service";
import { CellStoreService } from "./services/cell-store.service";
import { ChangeBackgroundService } from "./services/change-background.service";
import { EditMenuService } from "./services/edit-menu.service";
import { OrderStoreService } from "./services/order-store.service";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";
import { DragDropModule } from '@angular/cdk/drag-drop';
import { TranslateHttpLoader } from "@ngx-translate/http-loader";

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, environment.assets + 'assets/languages/');
}

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
        MatSnackBarModule,
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: httpTranslateLoaderFactory,
            deps: [HttpClient],
          },
        })
      ),
      OrderStoreService,
      CellStoreService,
      EditMenuService,
      ChangeBackgroundService,
      CellSettingsService,
      provideHttpClient(withInterceptorsFromDi()),
      provideAnimations(),
    ],
  }
