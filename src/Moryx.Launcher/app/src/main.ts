import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { NotificationsBar } from './app/notifications-bar/notifications-bar';

createApplication(appConfig)
  .then(appRef => {
    const LauncherElement = createCustomElement(App, {injector: appRef.injector});
    customElements.define('moryx-launcher', LauncherElement);

    const NotificationBarElement = createCustomElement(NotificationsBar, {injector: appRef.injector});
    customElements.define('moryx-notifications-bar', NotificationBarElement);
  })
  .catch((err) => console.error(err));
