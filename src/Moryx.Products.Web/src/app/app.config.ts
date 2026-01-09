import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom, provideExperimentalZonelessChangeDetection } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MAT_DIALOG_DEFAULT_OPTIONS, MatDialogModule } from "@angular/material/dialog";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatTableModule } from "@angular/material/table";
import { MatTabsModule } from "@angular/material/tabs";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatTreeModule } from "@angular/material/tree";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimations } from "@angular/platform-browser/animations";
import { AuthModule, ApiInterceptor, API_INTERCEPTOR_PROVIDER, AuthInterceptor, AUTH_INTERCEPTOR_PROVIDER, MoryxSnackbarService } from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { provideRouter, withComponentInputBinding, withRouterConfig } from "@angular/router";
import { routes } from "./app.routes";

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + "assets/languages/"
  );
}
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding(), withRouterConfig({ paramsInheritanceStrategy: 'always'})),
    importProvidersFrom(
      BrowserModule,
      FormsModule,
      MatTreeModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule,
      MatCardModule,
      MatListModule,
      MatTabsModule,
      MatIconModule,
      MatTableModule,
      MatProgressSpinnerModule,
      MatDialogModule,
      MatSelectModule,
      MatCheckboxModule,
      MatExpansionModule,
      MatSnackBarModule,
      MatToolbarModule,
      MatSidenavModule,
      MatMenuModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      AuthModule,
      MatProgressBarModule,
      MatSnackBarModule,
      MatTooltipModule,
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
    AuthInterceptor,
    AUTH_INTERCEPTOR_PROVIDER,
    MoryxSnackbarService,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};
