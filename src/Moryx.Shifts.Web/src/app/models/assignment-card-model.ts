import { ResourceModel } from "../api/models/Moryx/Operators/Endpoints/resource-model";
import { CalendarDate } from "./calendar-state";
import { OperatorModel, OperatorStatus } from "./operator-model";

export interface AssignmentCardModel{
    operator: OperatorModel ;
    notes: string | undefined;
    resource: ResourceModel ;
    priority: number ;
    status: OperatorStatus;
    id: number;
    days: CalendarDate[] ;
    shiftInstanceId: number;
    assignedDays: string;
}

export enum DayOfTheWeek {
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 0
}