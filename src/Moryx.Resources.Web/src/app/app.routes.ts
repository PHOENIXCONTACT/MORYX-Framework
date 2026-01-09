import { Routes } from "@angular/router";
import { DefaultDetailsViewComponent } from "./components/default-details-view/default-details-view.component";
import { DetailsViewComponent } from "./components/details-view/details-view.component";
import { ResourceMethodsComponent } from "./components/details-view/resource-methods/resource-methods.component";
import { ResourcePropertiesComponent } from "./components/details-view/resource-properties/resource-properties.component";
import { ResourceReferencesComponent } from "./components/details-view/resource-references/resource-references.component";
import { ShowDetailsGuard } from "./guards/show-details.guard";

export const routes: Routes =  [
  {
    path: 'details/:id',
    component: DetailsViewComponent,
    children: [
      { path: 'properties', component: ResourcePropertiesComponent, canActivate: [ShowDetailsGuard] },
      { path: 'references', component: ResourceReferencesComponent, canActivate: [ShowDetailsGuard] },
      {
        path: 'methods',
        component: ResourceMethodsComponent,
        canActivate: [ShowDetailsGuard],
      },
    ],
  },
  { path: '', component: DefaultDetailsViewComponent },
]