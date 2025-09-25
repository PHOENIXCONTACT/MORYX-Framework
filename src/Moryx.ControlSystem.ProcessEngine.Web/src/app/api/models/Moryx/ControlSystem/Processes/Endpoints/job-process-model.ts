/* tslint:disable */
/* eslint-disable */
import { ProcessActivityModel as MoryxControlSystemProcessesEndpointsProcessActivityModel } from '../../../../../models/Moryx/ControlSystem/Processes/Endpoints/process-activity-model';
import { ProcessProgress as MoryxControlSystemProcessesProcessProgress } from '../../../../../models/Moryx/ControlSystem/Processes/process-progress';
export interface JobProcessModel {
  activities?: Array<MoryxControlSystemProcessesEndpointsProcessActivityModel> | null;
  completed?: string;
  id?: number;
  isRunning?: boolean;
  recipeId?: number;
  rework?: boolean;
  started?: string;
  state?: MoryxControlSystemProcessesProcessProgress;
}
