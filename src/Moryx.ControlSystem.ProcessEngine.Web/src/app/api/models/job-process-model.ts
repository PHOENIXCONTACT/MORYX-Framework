/* tslint:disable */
/* eslint-disable */
import { ProcessActivityModel } from './process-activity-model';
import { ProcessProgress } from './process-progress';
export interface JobProcessModel {
  activities?: null | Array<ProcessActivityModel>;
  completed?: string;
  id?: number;
  isRunning?: boolean;
  recipeId?: number;
  rework?: boolean;
  started?: string;
  state?: ProcessProgress;
}
