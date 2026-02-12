/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { Availabilities } from "./availabilities/availabilities";
import { OperatorDetails } from "./operator-details/operator-details";
import { OperatorsManagement } from "./operators-management/operators-management";
import { SkillTypeDetails } from "./skill-type-details/skill-type-details";
import { SkillTypes } from "./skill-types/skill-types";
import { WorkstationOperators } from "./workstation-operators/workstation-operators";

export const routes: Routes = [
  { path: "workstations", component: WorkstationOperators },
  { path: "management", component: OperatorsManagement },
  { path: "availabilities", component: Availabilities },
  { path: "skill-types", component: SkillTypes },
  { path: "skill-types/:id", component: SkillTypeDetails },
  {
    path: "management/operator/details/:id",
    component: OperatorDetails,
  },
  { path: "", redirectTo: "workstations", pathMatch: "full" },
];

