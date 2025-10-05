/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { ProductInstanceState } from './product-instance-state';
export interface ProductInstanceModel {
  id?: number;
  properties?: Entry;
  state?: ProductInstanceState;
  type?: null | string;
}
