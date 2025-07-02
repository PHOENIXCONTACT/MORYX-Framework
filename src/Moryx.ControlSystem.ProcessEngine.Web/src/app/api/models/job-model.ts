/* tslint:disable */
/* eslint-disable */
import { ProductionJobModel } from './production-job-model';
import { SetupJobModel } from './setup-job-model';
export interface JobModel {
  canAbort?: boolean;
  canComplete?: boolean;
  displayState?: null | string;
  id?: number;
  isRunning?: boolean;
  isWaiting?: boolean;
  productionJob?: ProductionJobModel;
  recipeId?: number;
  setupJob?: SetupJobModel;
  state?: null | string;
  workplan?: null | string;
}
