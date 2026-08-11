/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

// Read the path base from a <meta> tag set by the server in _Layout.cshtml.
// Unlike modules (which call getPathBase("/Products") to strip their prefix from <base href>),
// the Launcher has no module prefix. Using getPathBase('') returned the full <base href>,
// which includes the current module's prefix (e.g. /Resources), causing wrong asset paths.
//
// Alternatives considered:
// - Web component input + effect: too late, environment is consumed at bootstrap (translate loader)
// - InjectionToken: same timing problem, input not available during app.config evaluation
// - Data attribute on <moryx-launcher>: works but couples environment to a specific element ID
// - Global variable via inline script: works but pollutes window
//
// A <meta> tag is standard HTML for page-level metadata, available before module scripts execute.
const pathBase = document.querySelector<HTMLMetaElement>('meta[name="moryx-path-base"]')
  ?.content?.replace(/\/+$/, '') ?? '';

export const environment = {
  production: true,
  assets: pathBase + "/_content/Moryx.Launcher/",
  rootUrl: pathBase
};
