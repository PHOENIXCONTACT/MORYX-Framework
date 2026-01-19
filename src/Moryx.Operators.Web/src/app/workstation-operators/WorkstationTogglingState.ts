/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { WorkstationViewModel } from '../models/workstation-view-model';

//workstation toggling state model
export interface WorkstationTogglingState {
  station: WorkstationViewModel;
  isExpanded: boolean;
}

