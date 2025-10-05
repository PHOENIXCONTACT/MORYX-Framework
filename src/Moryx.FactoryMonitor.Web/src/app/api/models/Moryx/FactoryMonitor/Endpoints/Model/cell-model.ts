/* tslint:disable */
/* eslint-disable */
import { CellDataModel as MoryxFactoryMonitorEndpointsModelCellDataModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-data-model';
import { CellLocationModel as MoryxFactoryMonitorEndpointsModelCellLocationModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
import { CellPropertySettings as MoryxFactoryMonitorEndpointsModelCellPropertySettings } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-property-settings';
export interface CellModel {
  cellData?: MoryxFactoryMonitorEndpointsModelCellDataModel;
  cellIconName?: null | string;
  cellImageURL?: null | string;
  cellLocation?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
  cellName?: null | string;
  cellPropertySettings?: null | {
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
};
  factoryId?: number;
  id?: number;
  identifier?: null | string;
}
