/* tslint:disable */
/* eslint-disable */
import { PartClassification as MoryxOrdersPartClassification } from '../../../../models/Moryx/Orders/part-classification';
import { StagingIndicator as MoryxOrdersStagingIndicator } from '../../../../models/Moryx/Orders/staging-indicator';
export interface ProductPartModel {
  classification?: MoryxOrdersPartClassification;
  id?: number;
  identifier?: string | null;
  name?: string | null;
  quantity?: number;
  stagingIndicator?: MoryxOrdersStagingIndicator;
  unit?: string | null;
}
