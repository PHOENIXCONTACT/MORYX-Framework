/* tslint:disable */
/* eslint-disable */
import { SetupJobClassification } from './setup-job-classification';
import { SetupStepModel } from './setup-step-model';
export interface SetupJobModel {
  classification?: SetupJobClassification;
  steps?: null | Array<SetupStepModel>;
  targetRecipeId?: number;
}
