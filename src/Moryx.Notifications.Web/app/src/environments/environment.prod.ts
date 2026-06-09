/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { getPathBase } from '@moryx/ngx-web-framework/environments';

let path_base = getPathBase("/Notifications");

export const environment = {
  production: true,
  assets: path_base + "/_content/Moryx.Notifications.Web/app/",
  rootUrl: path_base,
};

