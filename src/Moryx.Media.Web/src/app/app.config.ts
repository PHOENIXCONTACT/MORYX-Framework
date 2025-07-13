import { ClipboardModule } from "@angular/cdk/clipboard";
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
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
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER, MoryxSnackbarService } from "@moryx/ngx-web-framework";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { routes } from "./app.routes";
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function httpTranslateLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(
      http,
      environment.assets + 'assets/languages/'
    );
  }

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
            ApiModule.forRoot({ rootUrl: environment.rootUrl }), TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: httpTranslateLoaderFactory,
                deps: [HttpClient],
            },
        })),
        ApiInterceptor, API_INTERCEPTOR_PROVIDER, MoryxSnackbarService, provideHttpClient(withInterceptorsFromDi()),
        provideAnimations()
    ]
}