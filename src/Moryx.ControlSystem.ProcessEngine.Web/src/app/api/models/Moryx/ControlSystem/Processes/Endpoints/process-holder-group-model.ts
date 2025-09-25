/* tslint:disable */
/* eslint-disable */
import { ProcessHolderPositionModel as MoryxControlSystemProcessesEndpointsProcessHolderPositionModel } from '../../../../../models/Moryx/ControlSystem/Processes/Endpoints/process-holder-position-model';
import { VisualizationModel as MoryxControlSystemProcessesEndpointsVisualizationModel } from '../../../../../models/Moryx/ControlSystem/Processes/Endpoints/visualization-model';
export interface ProcessHolderGroupModel {
  id?: number;
  isEmpty?: boolean;
  name?: string | null;
  positions?: Array<MoryxControlSystemProcessesEndpointsProcessHolderPositionModel> | null;
  visualization?: MoryxControlSystemProcessesEndpointsVisualizationModel;
}
