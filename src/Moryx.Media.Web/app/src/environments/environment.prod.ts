/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { getPathBase } from @moryx/ngx-web-framework/environments;

let path_base = getPathBase("/Media");


export const environment = {
  production: true,
  assets: path_base + "/_content/Moryx.Media.Web/",
  rootUrl: path_base,
};
