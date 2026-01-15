/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Entry } from "@moryx/ngx-web-framework";

export interface SkillType{
    id: number;
    name: string;
    duration: string;
    acquiredCapabilities: Entry;
    trainedOperators: number;
}
