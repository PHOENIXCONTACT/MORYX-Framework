/* tslint:disable */
/* eslint-disable */
import { ProductionJobModel as MoryxControlSystemJobsEndpointsProductionJobModel } from '../../../../../models/Moryx/ControlSystem/Jobs/Endpoints/production-job-model';
import { SetupJobModel as MoryxControlSystemJobsEndpointsSetupJobModel } from '../../../../../models/Moryx/ControlSystem/Jobs/Endpoints/setup-job-model';
export interface JobModel {
  canAbort?: boolean;
  canComplete?: boolean;
  displayState?: string | null;
  id?: number;
  isRunning?: boolean;
  isWaiting?: boolean;
  productionJob?: MoryxControlSystemJobsEndpointsProductionJobModel;
  recipeId?: number;
  setupJob?: MoryxControlSystemJobsEndpointsSetupJobModel;
  state?: string | null;
  workplan?: string | null;
}
