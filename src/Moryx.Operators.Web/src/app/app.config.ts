import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { environment } from "src/environments/environment";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApiModule } from "./api/api.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatBadgeModule } from "@angular/material/badge";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCardModule } from "@angular/material/card";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { AppStoreService } from "./services/app-store.service";
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { provideRouter, withComponentInputBinding } from "@angular/router";
import { routes } from "./app.routes";

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + "assets/languages/"
  );
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding()),
    importProvidersFrom(
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      BrowserModule,
      BrowserModule,
      FormsModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule,
      MatIconModule,
      MatCardModule,
      MatListModule,
      MatSelectModule,
      MatSnackBarModule,
      MatProgressSpinnerModule,
      MatDatepickerModule,
      MatNativeDateModule,
      MatButtonToggleModule,
      MatTableModule,
      MatDialogModule,
      MatSidenavModule,
      MatToolbarModule,
      MatExpansionModule,
      MatTooltipModule,
      MatBadgeModule,
      MatMenuModule,
      ReactiveFormsModule,
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: httpTranslateLoaderFactory,
          deps: [HttpClient],
        },
      })
    ),
    AppStoreService,
    TranslateService,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};
