/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { JobsComponent } from "./app/components/jobs/jobs.component";
import { ProcessHoldersComponent } from "./app/components/process-holders/process-holders.component";

export const routes: Routes = [
  {
    path: "jobs",
    component: JobsComponent
  },
  {
    path: 'process-holders',
    component: ProcessHoldersComponent
  },
  {
    path: '', redirectTo: "jobs", pathMatch: 'full' 
  }
  ];
