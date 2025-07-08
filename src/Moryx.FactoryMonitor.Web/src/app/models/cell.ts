import { CellPropertySettings } from '../api/models/Moryx/FactoryMonitor/Endpoints/Model/cell-property-settings';
import { CellState } from '../api/models/Moryx/FactoryMonitor/Endpoints/Model/cell-state';
import { CellLocationModel } from '../api/models/Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
import { ActivityClassification } from '../api/models/Moryx/ControlSystem/Activities/activity-classification';
import { VisualizableItemModel } from '../api/models/Moryx/FactoryMonitor/Endpoints/Models/visualizable-item-model';



// flat cell model used in the entire UI
export default interface Cell extends VisualizableItemModel {
  image: null | string;
  name: string;
  propertySettings?: null | {
    [key: string]: CellPropertySettings;
  };
  state?: CellState | null;
  classification?: ActivityClassification | null;
  orderNumber: string;
  operationNumber: string;
  orderColor: string;
  factoryId: number;
}
