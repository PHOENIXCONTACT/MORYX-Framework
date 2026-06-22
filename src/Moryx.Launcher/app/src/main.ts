/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { NotificationsBar } from './app/notifications-bar/notifications-bar';
import { ModuleOverview } from './app/module-overview/module-overview';

createApplication(appConfig)
  .then(appRef => {
    const LauncherElement = createCustomElement(App, {injector: appRef.injector});
    customElements.define('moryx-launcher', LauncherElement);

    const NotificationBarElement = createCustomElement(NotificationsBar, {injector: appRef.injector});
    customElements.define('moryx-notifications-bar', NotificationBarElement);

    const ModuleOverviewElement = createCustomElement(ModuleOverview, {injector: appRef.injector});
    customElements.define('moryx-module-overview', ModuleOverviewElement);
  })
  .catch((err) => console.error(err));
