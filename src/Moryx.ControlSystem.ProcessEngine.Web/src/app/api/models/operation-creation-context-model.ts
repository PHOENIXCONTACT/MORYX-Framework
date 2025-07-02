/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { PartCreationContext } from './part-creation-context';
export interface OperationCreationContextModel {
  materialParameters?: null | Array<Entry>;
  name?: null | string;
  operationNumber?: null | string;
  orderNumber?: null | string;
  overDeliveryAmount?: number;
  parts?: null | Array<PartCreationContext>;
  plannedEnd?: string;
  plannedStart?: string;
  productIdentifier?: null | string;
  productName?: null | string;
  productRevision?: number;
  recipePreselection?: number;
  targetCycleTime?: number;
  targetStock?: null | string;
  totalAmount?: number;
  underDeliveryAmount?: number;
  unit?: null | string;
}
