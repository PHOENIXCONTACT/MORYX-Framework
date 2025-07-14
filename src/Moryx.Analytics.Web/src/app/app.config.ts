import { ApiInterceptor, API_INTERCEPTOR_PROVIDER, MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { provideHttpClient, withInterceptorsFromDi, HttpClient } from '@angular/common/http';
import { BrowserModule, HammerModule, bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { environment } from 'src/environments/environment';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { ApiModule } from './api/api.module';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + 'assets/languages/'
  );
}
export const appConfig: ApplicationConfig =
{
  providers: [
    importProvidersFrom(
      BrowserModule,
      FormsModule,
      MatProgressSpinnerModule,
      MatSidenavModule,
      MatSnackBarModule,
      MatToolbarModule,
      MatListModule,
      MatButtonModule,
      MatIconModule,
      MatDialogModule,
      MatFormFieldModule,
      MatInputModule,
      MatMenuModule,
      HammerModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
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
  ]
}
