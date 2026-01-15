/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AttendableResourceModel } from "../api/models/attendable-resource-model";
import { DayOfTheWeek } from "./assignment-card-model";
import { CalendarDate, CalendarState } from "./calendar-state";
import { OperatorModel } from "./operator-model";
import { ShiftCardModel } from "./shift-card-model";
import { ShiftInstanceModel } from "./shift-instance-model";

export default interface AssignmentData{
    operator?: OperatorModel;
    resource?: AttendableResourceModel;
    shift: ShiftCardModel;
    days: CalendarDate[];
    notes?: string | undefined;
    priority: number;
    calendarState: CalendarState
}
