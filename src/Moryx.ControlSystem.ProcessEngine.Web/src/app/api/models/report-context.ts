/* tslint:disable */
/* eslint-disable */
import { RestrictionDescription } from './restriction-description';
export interface ReportContext {
  canFinal?: boolean;
  canPartial?: boolean;
  reportedFailure?: number;
  reportedSuccess?: number;
  restrictions?: null | Array<RestrictionDescription>;
  scrapCount?: number;
  successCount?: number;
  unreportedFailure?: number;
  unreportedSuccess?: number;
}
