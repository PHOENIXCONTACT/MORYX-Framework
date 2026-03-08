/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { DefaultDetailsView } from "./components/default-details-view/default-details-view";
import { DetailsView } from "./components/details-view/details-view";
import { ResourceMethods } from "./components/details-view/resource-methods/resource-methods";
import { ResourceProperties } from "./components/details-view/resource-properties/resource-properties";
import { ResourceReferences } from "./components/details-view/resource-references/resource-references";

export const routes: Routes = [
  {
    path: 'details/:id',
    component: DetailsView,
    children: [
      { path: 'properties', component: ResourceProperties },
      { path: 'references', component: ResourceReferences },
      { path: 'methods', component: ResourceMethods },
    ],
  },
  { path: '', component: DefaultDetailsView },
]
