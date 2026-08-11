/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { OperationDocuments } from "./components/operation-documents/operation-documents";
import { OperationRecipes } from "./components/operation-recipes/operation-recipes";
import { Operations } from "./components/operations/operations";

export const routes: Routes = [
  { path: 'operations', component: Operations },
  { path: 'operation-recipes/:identifier', component: OperationRecipes },
  { path: 'operation-documents/:identifier', component: OperationDocuments },
  { path: '', redirectTo: 'operations', pathMatch: 'full' }
];
