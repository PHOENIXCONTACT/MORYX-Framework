/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { Constants } from '@app/constants';
import { NotificationsBar } from '@app/notifications-bar/notifications-bar';

(async () => {
  const app = await createApplication();

  const notificationsBar = createCustomElement(NotificationsBar, {
    injector: app.injector
  });

  customElements.define(Constants.WebComponentNames.NotificationsBar, notificationsBar);
})();
