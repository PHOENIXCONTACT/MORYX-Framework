import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { routes } from './app.routes';
import { API_INTERCEPTOR_PROVIDER, ApiInterceptor, AuthInterceptor, AuthModule, MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApiModule } from './api/api.module';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { environment } from '../environments/environment';

export function httpTranslateLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, environment.assets + 'languages/');
  }

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    importProvidersFrom(
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
          AuthModule,
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
        MoryxSnackbarService,
        provideHttpClient(withInterceptorsFromDi())
  ],
};
