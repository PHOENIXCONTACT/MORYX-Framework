import { Routes } from "@angular/router";
import { AvailabilitiesComponent } from "./availabilities/availabilities.component";
import { OperatorDetailsComponent } from "./operator-details/operator-details.component";
import { OperatorsManagementComponent } from "./operators-management/operators-management.component";
import { SkillTypeDetailsComponent } from "./skill-type-details/skill-type-details.component";
import { SkillTypesComponent } from "./skill-types/skill-types.component";
import { WorkstationOperatorsComponent } from "./workstation-operators/workstation-operators.component";

export const routes: Routes = [
  { path: "workstations", component: WorkstationOperatorsComponent },
  { path: "management", component: OperatorsManagementComponent },
  { path: "availabilities", component: AvailabilitiesComponent },
  { path: "skill-types", component: SkillTypesComponent },
  { path: "skill-types/:id", component: SkillTypeDetailsComponent },
  {
    path: "management/operator/details/:id",
    component: OperatorDetailsComponent,
  },
  { path: "", redirectTo: "workstations", pathMatch: "full" },
];
