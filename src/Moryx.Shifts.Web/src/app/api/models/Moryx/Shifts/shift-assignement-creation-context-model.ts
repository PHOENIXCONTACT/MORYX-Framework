/* tslint:disable */
/* eslint-disable */
import { AssignedDays as MoryxShiftsAssignedDays } from '../../../models/Moryx/Shifts/assigned-days';
export interface ShiftAssignementCreationContextModel {
  assignedDays?: MoryxShiftsAssignedDays;
  note?: string | null;
  operatorIdentifier?: string | null;
  priority?: number;
  resourceId?: number;
  shiftId?: number;
}
