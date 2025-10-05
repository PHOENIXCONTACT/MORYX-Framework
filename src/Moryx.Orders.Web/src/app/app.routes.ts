import { Routes } from "@angular/router";
import { OperationDocumentsComponent } from "./components/operation-documents/operation-documents.component";
import { OperationRecipesComponent } from "./components/operation-recipes/operation-recipes.component";
import { OperationsComponent } from "./components/operations/operations.component";

export const routes: Routes = [
  { path: 'operations', component: OperationsComponent },
  { path: 'operation-recipes/:identifier', component: OperationRecipesComponent },
  { path: 'operation-documents/:identifier', component: OperationDocumentsComponent },
  { path: '', redirectTo: 'operations', pathMatch: 'full' }
];