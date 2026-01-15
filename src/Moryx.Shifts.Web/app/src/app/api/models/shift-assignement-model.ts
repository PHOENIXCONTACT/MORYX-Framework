/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { AssignedDays } from '../models/assigned-days';
export interface ShiftAssignementModel {
  assignedDays?: AssignedDays;
  id?: number;
  note?: string | null;
  operatorIdentifier?: string | null;
  priority?: number;
  resourceId?: number;
  shiftId?: number;
}

