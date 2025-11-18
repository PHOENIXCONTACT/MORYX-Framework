/* tslint:disable */
/* eslint-disable */
import { AssignedDays } from '../models/assigned-days';
export interface ShiftAssignementCreationContextModel {
  assignedDays?: AssignedDays;
  note?: string | null;
  operatorIdentifier?: string | null;
  priority?: number;
  resourceId?: number;
  shiftId?: number;
}
