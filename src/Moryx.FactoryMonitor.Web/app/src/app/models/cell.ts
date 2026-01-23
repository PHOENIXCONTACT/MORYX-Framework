/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CellPropertySettings } from '../api/models/cell-property-settings';
import { CellState } from '../api/models/cell-state';
import { CellLocationModel } from '../api/models/cell-location-model';
import { ActivityClassification } from '../api/models/activity-classification';
import { VisualizableItemModel } from '../api/models/visualizable-item-model';



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

