/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export interface OperatorSkill {
    id: number;
    obtainedOn: Date,
    expiresOn: Date,
    typeId: number;
    operatorId: string;
    isExpired: boolean;
}
