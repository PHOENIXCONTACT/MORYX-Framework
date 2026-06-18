import {ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners} from '@angular/core';
import {provideRouter, withHashLocation} from '@angular/router';
import {MatIconRegistry} from '@angular/material/icon';

import {provideTranslateService} from '@ngx-translate/core';
import {provideTranslateHttpLoader} from '@ngx-translate/http-loader';
import {environment} from '../environments/environment';

import {routes} from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withHashLocation()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
      fallbackLang: 'en'
    }),
    provideAppInitializer(() => {
      inject(MatIconRegistry).setDefaultFontSetClass('material-symbols-outlined');
    }),
  ]
};
