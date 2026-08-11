/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { VariantOverview } from "./components/variant-overview/variant-overview";
import { MediaOverview } from "./components/media-overview/media-overview";

export const routes: Routes = [
    { path: 'details/:id', component: VariantOverview },
    { path: '', component: MediaOverview },
  ];

