/* tslint:disable */
/* eslint-disable */
import { SetupJobClassification as MoryxControlSystemJobsEndpointsSetupJobClassification } from '../../../../../models/Moryx/ControlSystem/Jobs/Endpoints/setup-job-classification';
import { SetupStepModel as MoryxControlSystemJobsEndpointsSetupStepModel } from '../../../../../models/Moryx/ControlSystem/Jobs/Endpoints/setup-step-model';
export interface SetupJobModel {
  classification?: MoryxControlSystemJobsEndpointsSetupJobClassification;
  steps?: Array<MoryxControlSystemJobsEndpointsSetupStepModel> | null;
  targetRecipeId?: number;
}
