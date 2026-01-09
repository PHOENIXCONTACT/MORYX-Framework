import { Time } from "@angular/common";
import { DayOfTheWeek } from "./assignment-card-model";

export interface ShiftCardModel{
    id: number;
    shiftName: string ;
    startTime: Time;
    endTime: Time;
    startDate: Date;
    endDate: Date;
}


