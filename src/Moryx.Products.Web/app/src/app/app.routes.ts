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
import { SearchResult } from "./components/search-result/search-result";
import { ProductResolver } from "./components/products-details-view/products-resolver";
import { WorkInProgressGuard } from "./app-guard";
import { ReferencesResolver } from "./components/products-details-view/product-references/references-resolver";
import { RecipeResolver } from "./components/products-details-view/product-recipes/product-recipes-details/recipe-resolver";
import { PartsResolver } from "./components/products-details-view/product-parts/part-resolver";

export const routes: Routes = [
  {
    path: 'details/:id',
    component: ProductsDetailsView,
    resolve: {
      product: ProductResolver
    },
    children: [
      { path: '', redirectTo: 'properties', pathMatch: 'full' },
      { path: 'properties', component: ProductProperties },
      { path: 'references', component: ProductReferences, resolve: { references: ReferencesResolver } },
      { path: 'parts/:partName/:partId', component: ProductParts, resolve: { references: PartsResolver }},
      {
        path: 'recipes',
        component: ProductRecipes,
        children: [
          { path: '', component: DefaultView, pathMatch: 'full' },
          { path: ':recipeId', component: ProductRecipesDetails, resolve: { references: RecipeResolver } },
        ],
      },
    ],
  },
  { path: '', component: DefaultView, pathMatch: 'full', canActivate: [WorkInProgressGuard] },
  { path: 'import/:importer', component: ProductsImporter },
  { path: 'search', component: SearchResult }
]
