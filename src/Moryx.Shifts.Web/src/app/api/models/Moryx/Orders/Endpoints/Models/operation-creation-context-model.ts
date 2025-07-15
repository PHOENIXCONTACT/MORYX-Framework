/* tslint:disable */
/* eslint-disable */
import { PartCreationContext as MoryxOrdersPartCreationContext } from '../../../../../models/Moryx/Orders/part-creation-context';
import { Entry as MoryxSerializationEntry } from '../../../../../models/Moryx/Serialization/entry';
export interface OperationCreationContextModel {
  materialParameters?: Array<MoryxSerializationEntry> | null;
  name?: string | null;
  operationNumber?: string | null;
  orderNumber?: string | null;
  overDeliveryAmount?: number;
  parts?: Array<MoryxOrdersPartCreationContext> | null;
  plannedEnd?: string;
  plannedStart?: string;
  productIdentifier?: string | null;
  productName?: string | null;
  productRevision?: number;
  recipePreselection?: number;
  targetCycleTime?: number;
  targetStock?: string | null;
  totalAmount?: number;
  underDeliveryAmount?: number;
  unit?: string | null;
}
