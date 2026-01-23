/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export interface ProcessHolderPosition {
    id: number;
    groupId: number;
    name: string;
    position: number;
    activity: string;
    order: string;
    process: string;
    isEmpty: boolean;
}
