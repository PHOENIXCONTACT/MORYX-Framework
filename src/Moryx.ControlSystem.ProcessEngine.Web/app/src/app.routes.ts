/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { Jobs } from "./app/components/jobs/jobs";
import { ProcessHolders } from "./app/components/process-holders/process-holders";

export const routes: Routes = [
  {
    path: "jobs",
    component: Jobs
  },
  {
    path: 'process-holders',
    component: ProcessHolders
  },
  {
    path: '', redirectTo: "jobs", pathMatch: 'full' 
  }
  ];
