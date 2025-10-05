/* tslint:disable */
/* eslint-disable */
import { PropertyFilter } from './property-filter';
import { RecipeFilter } from './recipe-filter';
import { RevisionFilter } from './revision-filter';
import { Selector } from './selector';
export interface ProductQuery {
  excludeDerivedTypes?: boolean;
  identifier?: null | string;
  includeDeleted?: boolean;
  name?: null | string;
  propertyFilters?: null | Array<PropertyFilter>;
  recipeFilter?: RecipeFilter;
  revision?: number;
  revisionFilter?: RevisionFilter;
  selector?: Selector;
  type?: null | string;
}
