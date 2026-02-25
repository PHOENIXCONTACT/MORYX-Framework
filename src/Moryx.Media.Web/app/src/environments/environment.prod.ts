/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

let node = document.head.getElementsByTagName("meta")?.namedItem("moryx-pathbase");
let path_base = node?.content ?? "";


export const environment = {
  production: true,
  assets: path_base + "/_content/Moryx.Media.Web/",
  rootUrl: path_base,
};
