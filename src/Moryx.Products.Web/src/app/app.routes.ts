import { Routes } from "@angular/router";
import { DefaultViewComponent } from "./components/default-view/default-view.component";
import { ProductPartsComponent } from "./components/products-details-view/product-parts/product-parts.component";
import { ProductPropertiesComponent } from "./components/products-details-view/product-properties/product-properties.component";
import { ProductRecipesDetailsComponent } from "./components/products-details-view/product-recipes/product-recipes-details/product-recipes-details.component";
import { ProductRecipesComponent } from "./components/products-details-view/product-recipes/product-recipes.component";
import { ProductReferencesComponent } from "./components/products-details-view/product-references/product-references.component";
import { ProductsDetailsViewComponent } from "./components/products-details-view/products-details-view.component";
import { ProductsImporterComponent } from "./components/products-importer/products-importer.component";
import { SearchResultComponent } from "./components/search-result/search-result.component";
import { ImporterGuard } from "./guards/importer.guard";

export const routes: Routes = [
    {
        path: 'details/:id',
        component: ProductsDetailsViewComponent,
        children: [
          { path: 'properties', component: ProductPropertiesComponent },
          { path: 'references', component: ProductReferencesComponent },
          {
            path: 'parts/:partName/:partId',
            component: ProductPartsComponent,
          },
          {
            path: 'recipes',
            component: ProductRecipesComponent,
            children: [
              { path: '', component: DefaultViewComponent, pathMatch: 'full' },
              {
                path: ':recipeId',
                component: ProductRecipesDetailsComponent,
              },
            ],
          },
        ],
      },
      { path: '', component: DefaultViewComponent, pathMatch: 'full' },
      {
        path: 'import/:importer',
        component: ProductsImporterComponent,
        canActivate: [ImporterGuard]
      },
      {path: 'search', component: SearchResultComponent}
]