/* tslint:disable */
/* eslint-disable */
import { CellLocationModel as MoryxFactoryMonitorEndpointsModelCellLocationModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
import { CellModel as MoryxFactoryMonitorEndpointsModelCellModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-model';
import { OrderModel as MoryxFactoryMonitorEndpointsModelOrderModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/order-model';
import { ActivityChangedModel as MoryxFactoryMonitorEndpointsModelsActivityChangedModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Models/activity-changed-model';
import { CellStateChangedModel as MoryxFactoryMonitorEndpointsModelsCellStateChangedModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Models/cell-state-changed-model';
import { ResourceChangedModel as MoryxFactoryMonitorEndpointsModelsResourceChangedModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Models/resource-changed-model';
export interface FactoryStateModel {
  activityChangedModels?: null | Array<MoryxFactoryMonitorEndpointsModelsActivityChangedModel>;
  backgroundURL?: null | string;
  cellStateChangedModels?: null | Array<MoryxFactoryMonitorEndpointsModelsCellStateChangedModel>;
  /** @deprecated */cells?: null | Array<MoryxFactoryMonitorEndpointsModelCellModel>;
  hasManufacturingFactory?: boolean;
  iconName?: null | string;
  id?: number;
  isACell?: boolean;
  location?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
  orderModels?: null | Array<MoryxFactoryMonitorEndpointsModelOrderModel>;
  resourceChangedModels?: null | Array<MoryxFactoryMonitorEndpointsModelsResourceChangedModel>;
}
