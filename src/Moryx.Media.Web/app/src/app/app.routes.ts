/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { VariantOverviewComponent } from "./components/variant-overview/variant-overview.component";
import { MediaOverviewComponent } from "./components/media-overview/media-overview.component";

export const routes: Routes = [
    { path: 'details/:id', component: VariantOverviewComponent },
    { path: '', component: MediaOverviewComponent },
  ];
  
