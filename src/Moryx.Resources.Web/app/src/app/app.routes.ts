/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { DefaultDetailsViewComponent } from "./components/default-details-view/default-details-view.component";
import { DetailsViewComponent } from "./components/details-view/details-view.component";
import { ResourceMethodsComponent } from "./components/details-view/resource-methods/resource-methods.component";
import { ResourcePropertiesComponent } from "./components/details-view/resource-properties/resource-properties.component";
import { ResourceReferencesComponent } from "./components/details-view/resource-references/resource-references.component";

export const routes: Routes = [
  {
    path: 'details/:id',
    component: DetailsViewComponent,
    children: [
      { path: 'properties', component: ResourcePropertiesComponent },
      { path: 'references', component: ResourceReferencesComponent },
      { path: 'methods', component: ResourceMethodsComponent },
    ],
  },
  { path: '', component: DefaultDetailsViewComponent },
]
