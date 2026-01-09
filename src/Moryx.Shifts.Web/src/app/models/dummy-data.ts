import { AttendableResourceModel } from '../api/models/attendable-resource-model';
import { AssignmentCardModel, DayOfTheWeek } from './assignment-card-model';
import { OperatorModel, OperatorStatus } from './operator-model';
import { OrderModel } from './order-model';
import { ShiftCardModel } from './shift-card-model';
import { ShiftInstanceModel } from './shift-instance-model';
import { ShiftTypeModel } from './shift-type-model';

export const SHIFT_TYPES: ShiftTypeModel[] = [
  {
    id: 1,
    duration: 7,
    name: 'Early Shift',
    startTime: { hours: 6, minutes: 0 },
    endTime: { hours: 14, minutes: 0 },
  },
  {
    id: 2,
    duration: 7,
    name: 'Day Shift',
    startTime: { hours: 14, minutes: 0 },
    endTime: { hours: 22, minutes: 0 },
  },
  {
    id: 3,
    duration: 7,
    name: 'Night Shift',
    startTime: { hours: 22, minutes: 0 },
    endTime: { hours: 6, minutes: 0 },
  }
];

export const SHIFT_INSTANCES: ShiftInstanceModel[] = [
  {
    id: 1,
    shiftType: SHIFT_TYPES[0],
    startDate: new Date("06/16/2024"),
    endDate: new Date("06/22/2024"),
  },
  {
    id: 2,
    shiftType: SHIFT_TYPES[1],
    startDate: new Date("06/16/2024"),
    endDate: new Date("06/22/2024"),
  },
  {
    id: 3,
    shiftType: SHIFT_TYPES[2],
    startDate: new Date("06/16/2024"),
    endDate: new Date("06/22/2024"),
  }
];

export const SHIFTS: ShiftCardModel[] = [
  {
    id: SHIFT_INSTANCES[0]?.id ?? 1,
    shiftName: SHIFT_INSTANCES[0].shiftType.name,
    startDate: SHIFT_INSTANCES[0].startDate,
    endDate: SHIFT_INSTANCES[0].endDate,
    startTime: SHIFT_INSTANCES[0].shiftType.startTime,
    endTime: SHIFT_INSTANCES[0].shiftType.endTime,
  },
  {
    id: SHIFT_INSTANCES[1]?.id ?? 1,
    shiftName: SHIFT_INSTANCES[1].shiftType.name,
    startDate: SHIFT_INSTANCES[1].startDate,
    endDate: SHIFT_INSTANCES[1].endDate,
    startTime: SHIFT_INSTANCES[1].shiftType.startTime,
    endTime: SHIFT_INSTANCES[1].shiftType.endTime,
  },
  {
    id: SHIFT_INSTANCES[2]?.id ?? 1,
    shiftName: SHIFT_INSTANCES[2].shiftType.name,
    startDate: SHIFT_INSTANCES[2].startDate,
    endDate: SHIFT_INSTANCES[2].endDate,
    startTime: SHIFT_INSTANCES[2].shiftType.startTime,
    endTime: SHIFT_INSTANCES[2].shiftType.endTime,
  }
];

export const OPERATORS: OperatorModel[] = [
  { id: "1", name: 'Schichtfu 1', status: OperatorStatus.Available },
  { id: "2", name: 'Operator 1', status: OperatorStatus.Available },
  { id: "3", name: 'Operator 2', status: OperatorStatus.OnVacation },
  { id: "4", name: 'Operator 3', status: OperatorStatus.OnVacation },
  { id: "5", name: 'Operator 4', status: OperatorStatus.Available },
  { id: "6", name: 'Operator 5', status: OperatorStatus.NotAllowed },
  { id:" 7", name: 'Operator 6', status: OperatorStatus.NotQualified },
  { id: "8", name: 'Operator 7', status: OperatorStatus.Available },
];

export const RESOURCES: AttendableResourceModel[] = [
  { id: 1, name: 'Building 1' },
  { id: 2, name: 'Komax' },
  { id: 3, name: 'HM' },
  { id: 4, name: 'Monteur' },
  { id: 5, name: 'BM018337' },
  { id: 6, name: 'BM009833' },
  { id: 7, name: 'BM008786' },
  { id: 8, name: 'Elektrofachkraft' },
];

export const ASSIGMENTS: AssignmentCardModel[] = [
];

export const ORDERS: OrderModel[] = [
  {
    orderNumber: "1000001",
    operationNumber: "1000",
    totalHours: 20,
    date: new Date("06/17/2024")
  },
  {
    orderNumber: "1000002",
    operationNumber: "1001",
    totalHours: 10,
    date: new Date("06/17/2024")
  },
  {
    orderNumber: "1000003",
    operationNumber: "1002",
    totalHours: 20,
    date: new Date("06/17/2024")
  }
  ,
  {
    orderNumber: "1000004",
    operationNumber: "1003",
    totalHours: 10,
    date: new Date("06/18/2024")
  }
  ,
  {
    orderNumber: "1000005",
    operationNumber: "1004",
    totalHours: 5,
    date: new Date("06/18/2024")
  }
];