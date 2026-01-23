/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

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



