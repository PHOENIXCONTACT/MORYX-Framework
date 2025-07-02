/* tslint:disable */
/* eslint-disable */
import { RestrictionDescription } from './restriction-description';
export interface BeginContext {
  canBegin?: boolean;
  partialAmount?: number;
  residualAmount?: number;
  restrictions?: null | Array<RestrictionDescription>;
  scrapCount?: number;
  successCount?: number;
}
