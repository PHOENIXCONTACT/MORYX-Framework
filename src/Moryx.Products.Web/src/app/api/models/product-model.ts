/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { PartConnector } from './part-connector';
import { ProductState } from './product-state';
import { RecipeModel } from './recipe-model';
export interface ProductModel {
  id?: number;
  identifier?: null | string;
  name?: null | string;
  parts?: null | Array<PartConnector>;
  properties?: Entry;
  recipes?: null | Array<RecipeModel>;
  revision?: number;
  state?: ProductState;
  type?: null | string;
}
