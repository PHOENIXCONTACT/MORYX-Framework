/* tslint:disable */
/* eslint-disable */
import { PartClassification } from './part-classification';
import { StagingIndicator } from './staging-indicator';
export interface PartCreationContext {
  classification?: PartClassification;
  name?: null | string;
  number?: null | string;
  quantity?: number;
  stagingIndicator?: StagingIndicator;
  unit?: null | string;
}
