/* tslint:disable */
/* eslint-disable */
import { CellLocationModel as MoryxFactoryMonitorEndpointsModelCellLocationModel } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
import { CellPropertySettings as MoryxFactoryMonitorEndpointsModelCellPropertySettings } from '../../../../Moryx/FactoryMonitor/Endpoints/Model/cell-property-settings';
export interface ResourceChangedModel {
  cellIconName?: null | string;
  cellImageURL?: null | string;
  cellLocation?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
  cellName?: null | string;
  cellPropertySettings?: null | {
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
};
  factoryId?: number;
  iconName?: null | string;
  id?: number;
  isACell?: boolean;
  location?: MoryxFactoryMonitorEndpointsModelCellLocationModel;
}
