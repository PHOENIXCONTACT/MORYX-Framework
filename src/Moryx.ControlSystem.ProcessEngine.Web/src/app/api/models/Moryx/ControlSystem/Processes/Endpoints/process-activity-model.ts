/* tslint:disable */
/* eslint-disable */
import { ActivityClassification as MoryxControlSystemActivitiesActivityClassification } from '../../../../../models/Moryx/ControlSystem/Activities/activity-classification';
import { ActivityResourceModel as MoryxControlSystemProcessesEndpointsActivityResourceModel } from '../../../../../models/Moryx/ControlSystem/Processes/Endpoints/activity-resource-model';
import { TracingModel as MoryxControlSystemProcessesEndpointsTracingModel } from '../../../../../models/Moryx/ControlSystem/Processes/Endpoints/tracing-model';
export interface ProcessActivityModel {
  classification?: MoryxControlSystemActivitiesActivityClassification;
  id?: number;
  instanceId?: number | null;
  isCompleted?: boolean;
  possibleResources?: Array<MoryxControlSystemProcessesEndpointsActivityResourceModel> | null;
  requiredCapabilities?: string | null;
  resource?: MoryxControlSystemProcessesEndpointsActivityResourceModel;
  result?: number | null;
  resultName?: string | null;
  state?: string | null;
  tracing?: MoryxControlSystemProcessesEndpointsTracingModel;
  type?: string | null;
}
