import { AssignableOperator } from "../api/models/Moryx/Operators/assignable-operator";

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