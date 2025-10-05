import { ResourceModel } from "../api/models/Moryx/Operators/Endpoints/resource-model";
import { DayOfTheWeek } from "./assignment-card-model";
import { CalendarDate, CalendarState } from "./calendar-state";
import { OperatorModel } from "./operator-model";
import { ShiftCardModel } from "./shift-card-model";
import { ShiftInstanceModel } from "./shift-instance-model";

export default interface AssignmentData{
    operator?: OperatorModel;
    resource?: ResourceModel;
    shift: ShiftCardModel;
    days: CalendarDate[];
    notes?: string | undefined;
    priority: number;
    calendarState: CalendarState
}