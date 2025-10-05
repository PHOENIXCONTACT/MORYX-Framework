/* tslint:disable */
/* eslint-disable */
import { ActivityClassification as MoryxControlSystemActivitiesActivityClassification } from '../../../../Moryx/ControlSystem/Activities/activity-classification';
import { OrderReferenceModel as MoryxFactoryMonitorEndpointsModelsOrderReferenceModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Models/order-reference-model';
export interface ActivityChangedModel {
  classification?: MoryxControlSystemActivitiesActivityClassification;
  id?: number;
  orderReferenceModel?: MoryxFactoryMonitorEndpointsModelsOrderReferenceModel;
  resourceId?: number;
}
