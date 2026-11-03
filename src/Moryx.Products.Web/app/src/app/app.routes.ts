/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { DefaultView } from "./components/default-view/default-view";
import { ProductParts } from "./components/products-details-view/product-parts/product-parts";
import { ProductProperties } from "./components/products-details-view/product-properties/product-properties";
import { ProductRecipesDetails } from "./components/products-details-view/product-recipes/product-recipes-details/product-recipes-details";
import { ProductRecipes } from "./components/products-details-view/product-recipes/product-recipes";
import { ProductReferences } from "./components/products-details-view/product-references/product-references";
import { ProductsDetailsView } from "./components/products-details-view/products-details-view";
import { ProductsImporter } from "./components/products-importer/products-importer";
import { WorkInProgressGuard } from "./app-guard";
import { CancellationComponent } from "./components/cancellation/cancellation.component";

export const routes: Routes = [
  {
    path: 'details/:id',
    component: ProductsDetailsView,
    children: [
      { path: '', redirectTo: 'properties', pathMatch: 'full' },
      { path: 'properties', component: ProductProperties },
      { path: 'references', component: ProductReferences },
      { path: 'parts/:partName/:partId', component: ProductParts },
      {
        path: 'recipes',
        component: ProductRecipes,
        children: [
          { path: '', component: DefaultView, pathMatch: 'full' },
          { path: ':recipeId', component: ProductRecipesDetails },
        ],
      },
    ],
  },
  { path: '', component: DefaultView, pathMatch: 'full', canActivate: [WorkInProgressGuard] },
  { path: 'import/:importer', component: ProductsImporter },
  { path: 'cancel', component: CancellationComponent }
]
