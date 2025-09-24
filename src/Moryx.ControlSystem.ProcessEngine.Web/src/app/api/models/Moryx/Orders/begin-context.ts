/* tslint:disable */
/* eslint-disable */
import { RestrictionDescription as MoryxOrdersRestrictionsRestrictionDescription } from '../../../models/Moryx/Orders/Restrictions/restriction-description';
export interface BeginContext {
  canBegin?: boolean;
  canReduce?: boolean;
  minimalTargetAmount?: number;
  partialAmount?: number;
  residualAmount?: number;
  restrictions?: Array<MoryxOrdersRestrictionsRestrictionDescription> | null;
  scrapCount?: number;
  successCount?: number;
}
