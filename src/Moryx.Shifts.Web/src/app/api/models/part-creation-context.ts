/* tslint:disable */
/* eslint-disable */
import { PartClassification } from '../models/part-classification';
import { StagingIndicator } from '../models/staging-indicator';
export interface PartCreationContext {
  classification?: PartClassification;
  name?: string | null;
  number?: string | null;
  quantity?: number;
  stagingIndicator?: StagingIndicator;
  unit?: string | null;
}
