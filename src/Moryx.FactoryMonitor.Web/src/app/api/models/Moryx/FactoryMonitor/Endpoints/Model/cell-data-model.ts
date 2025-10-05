/* tslint:disable */
/* eslint-disable */
import { CellState as MoryxFactoryMonitorEndpointsModelCellState } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-state';
import { ProcessModel as MoryxFactoryMonitorEndpointsModelProcessModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/process-model';
import { TransportPathModel as MoryxFactoryMonitorEndpointsModelTransportPathModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/transport-path-model';
export interface CellDataModel {
  forkliftDispatchRoutes?: null | Array<MoryxFactoryMonitorEndpointsModelTransportPathModel>;
  processActivity?: MoryxFactoryMonitorEndpointsModelProcessModel;
  state?: MoryxFactoryMonitorEndpointsModelCellState;
}
