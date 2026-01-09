import { Routes } from "@angular/router";
import { VariantOverviewComponent } from "./components/variant-overview/variant-overview.component";
import { MediaOverviewComponent } from "./components/media-overview/media-overview.component";

export const routes: Routes = [
    { path: 'details/:id', component: VariantOverviewComponent },
    { path: '', component: MediaOverviewComponent },
  ];
  