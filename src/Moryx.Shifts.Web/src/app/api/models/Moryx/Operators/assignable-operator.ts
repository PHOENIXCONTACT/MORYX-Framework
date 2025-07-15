/* tslint:disable */
/* eslint-disable */
import { IOperatorAssignable as MoryxOperatorsIOperatorAssignable } from '../../../models/Moryx/Operators/i-operator-assignable';
export interface AssignableOperator {
  assignedResources?: Array<MoryxOperatorsIOperatorAssignable> | null;
  firstName?: string | null;
  identifier?: string | null;
  lastName?: string | null;
  pseudonym?: string | null;
  signedIn?: boolean;
}
