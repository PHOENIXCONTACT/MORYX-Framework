/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { PropertyFilterOperator } from './property-filter-operator';
export interface PropertyFilter {
  entry?: Entry;
  operator?: PropertyFilterOperator;
}
