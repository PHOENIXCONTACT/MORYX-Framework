/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ShiftTypeModel } from './shift-type-model';

export interface ShiftInstanceModel {
  id: number;
  shiftType: ShiftTypeModel;
  startDate: Date;
  endDate: Date;
}

