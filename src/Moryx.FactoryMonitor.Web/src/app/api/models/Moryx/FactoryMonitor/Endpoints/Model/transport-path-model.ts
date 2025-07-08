/* tslint:disable */
/* eslint-disable */
import { Position as MoryxFactoryPosition } from '../../../../Moryx/Factory/position';
import { CellLocationModel as MoryxFactoryMonitorEndpointsModelCellLocationModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
export interface TransportPathModel {
  destination?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
  origin?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
  wayPoints?: null | Array<MoryxFactoryPosition>;
}
