/* tslint:disable */
/* eslint-disable */
import { PartClassification } from './part-classification';
import { StagingIndicator } from './staging-indicator';
export interface ProductPartModel {
  classification?: PartClassification;
  id?: number;
  identifier?: null | string;
  name?: null | string;
  quantity?: number;
  stagingIndicator?: StagingIndicator;
  unit?: null | string;
}
