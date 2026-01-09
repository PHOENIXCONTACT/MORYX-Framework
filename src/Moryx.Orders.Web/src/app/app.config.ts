import {
  HttpClient,
  provideHttpClient,
  withInterceptorsFromDi,
} from "@angular/common/http";
import {
  ApplicationConfig,
  enableProdMode,
  importProvidersFrom,
} from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatBadgeModule } from "@angular/material/badge";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatChipsModule } from "@angular/material/chips";
import { MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatToolbarModule } from "@angular/material/toolbar";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import {
  ApiInterceptor,
  API_INTERCEPTOR_PROVIDER,
  MoryxSnackbarService,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + "assets/languages/"
  );
}

if (environment.production) {
  enableProdMode();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      MatExpansionModule,
      MatGridListModule,
      MatProgressBarModule,
      MatBadgeModule,
      MatButtonModule,
      MatDialogModule,
      MatInputModule,
      FormsModule,
      MatListModule,
      MatIconModule,
      MatCardModule,
      MatFormFieldModule,
      MatSelectModule,
      MatSnackBarModule,
      MatRadioModule,
      MatProgressSpinnerModule,
      MatTableModule,
      MatChipsModule,
      MatSidenavModule,
      MatToolbarModule,
      NgxDocViewerModule,
      ReactiveFormsModule,
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: httpTranslateLoaderFactory,
          deps: [HttpClient],
        },
      })
    ),
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    MoryxSnackbarService,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};
