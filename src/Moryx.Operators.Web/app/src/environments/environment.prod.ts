/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

function getPathBase(modulePrefix: string) {
  
  let baseElement = document.querySelector('base');
  let href = baseElement?.href; // routingPrefix/commandcenter
  if(modulePrefix == null || modulePrefix == undefined || modulePrefix == "")
    return href;

  if(!modulePrefix.startsWith('/')) {
    modulePrefix = '/' + modulePrefix;
  }
  if(href?.endsWith(modulePrefix)) {
    return href.substring(0, href.length - modulePrefix.length);  // routingPrefix
  }
  throw Error("Not implemented");
}


let path_base = getPathBase("/Operators");

export const environment = {
  production: true,
  assets: path_base +  "/_content/Moryx.Operators.Web/",
  rootUrl: path_base,
  ignoreIam: true,
};
