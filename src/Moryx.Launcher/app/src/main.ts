import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { appConfig } from './app/app.config';
import { App } from './app/app';

createApplication(appConfig)
  .then(appRef => {
    const LauncherElement = createCustomElement(App, { injector: appRef.injector });
    customElements.define('moryx-launcher', LauncherElement);
  })
  .catch((err) => console.error(err));
