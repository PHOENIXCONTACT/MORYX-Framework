/* tslint:disable */
/* eslint-disable */
import { RestrictionDescription as MoryxOrdersRestrictionsRestrictionDescription } from '../../../models/Moryx/Orders/Restrictions/restriction-description';
export interface ReportContext {
  canFinal?: boolean;
  canPartial?: boolean;
  reportedFailure?: number;
  reportedSuccess?: number;
  restrictions?: Array<MoryxOrdersRestrictionsRestrictionDescription> | null;
  scrapCount?: number;
  successCount?: number;
  unreportedFailure?: number;
  unreportedSuccess?: number;
}
