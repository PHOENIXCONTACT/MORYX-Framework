/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Time } from "@angular/common";

export interface ShiftTypeModel {
    id: number;
    name: string
    duration: number;
    startTime: Time;
    endTime: Time;
}
