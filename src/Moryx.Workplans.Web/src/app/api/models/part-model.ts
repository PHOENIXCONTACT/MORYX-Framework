/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { ProductModel } from './product-model';
export interface PartModel {
  id?: number;
  product?: ProductModel;
  properties?: Entry;
}
