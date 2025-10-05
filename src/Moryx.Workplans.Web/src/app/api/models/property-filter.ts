/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { PropertyFilterOperator } from './property-filter-operator';
export interface PropertyFilter {
  entry?: Entry;
  operator?: PropertyFilterOperator;
}
