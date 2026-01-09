import { Time } from "@angular/common";

export interface ShiftTypeModel {
    id: number;
    name: string
    duration: number;
    startTime: Time;
    endTime: Time;
}