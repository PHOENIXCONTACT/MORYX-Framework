/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { PartConnector } from './part-connector';
import { ProductFileModel } from './product-file-model';
import { ProductState } from './product-state';
import { RecipeModel } from './recipe-model';
export interface ProductModel {
  files?: null | Array<ProductFileModel>;
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
