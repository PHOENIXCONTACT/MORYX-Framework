/* tslint:disable */
/* eslint-disable */
import { ActivityClassification as MoryxControlSystemActivitiesActivityClassification } from '../../../../Moryx/ControlSystem/Activities/activity-classification';
import { ActivityResourceModel as MoryxControlSystemProcessesEndpointsActivityResourceModel } from '../../../../Moryx/ControlSystem/Processes/Endpoints/activity-resource-model';
import { TracingModel as MoryxControlSystemProcessesEndpointsTracingModel } from '../../../../Moryx/ControlSystem/Processes/Endpoints/tracing-model';
import { OrderModel as MoryxFactoryMonitorEndpointsModelOrderModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/order-model';
export interface ProcessModel {
  classification?: MoryxControlSystemActivitiesActivityClassification;
  id?: number;
  instanceId?: null | number;
  isCompleted?: boolean;
  order?: MoryxFactoryMonitorEndpointsModelOrderModel;
  possibleResources?: null | Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>;
  requiredCapabilities?: null | string;
  resource?: MoryxControlSystemProcessesEndpointsActivityResourceModel;
  result?: null | number;
  resultName?: null | string;
  state?: null | string;
  tracing?: MoryxControlSystemProcessesEndpointsTracingModel;
  type?: null | string;
}
