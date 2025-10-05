/* tslint:disable */
/* eslint-disable */
import { ProductDefinitionModel } from './product-definition-model';
import { ProductImporter } from './product-importer';
import { RecipeDefinitionModel } from './recipe-definition-model';
export interface ProductCustomization {
  importers?: null | Array<ProductImporter>;
  productTypes?: null | Array<ProductDefinitionModel>;
  recipeTypes?: null | Array<RecipeDefinitionModel>;
}
