/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AssignableOperator } from "../api/models/assignable-operator";

export interface OperatorModel extends AssignableOperator {
    id: string;
    name: string;
    status:  OperatorStatus
}

export enum OperatorStatus {
    Available,
    OnVacation,
    NotAllowed,
    NotQualified
}

export function instanceOfOperator(object: any): object is OperatorModel {
    const result = 'status' in object;
    return result;
}
