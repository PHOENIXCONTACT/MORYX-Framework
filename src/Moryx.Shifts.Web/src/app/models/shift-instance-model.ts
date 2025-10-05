import { ShiftTypeModel } from './shift-type-model';

export interface ShiftInstanceModel {
  id: number;
  shiftType: ShiftTypeModel;
  startDate: Date;
  endDate: Date;
}
