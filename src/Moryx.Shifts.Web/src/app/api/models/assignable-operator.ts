/* tslint:disable */
/* eslint-disable */
import { IOperatorAssignable } from '../models/i-operator-assignable';
export interface AssignableOperator {
  assignedResources?: Array<IOperatorAssignable> | null;
  firstName?: string | null;
  identifier?: string | null;
  lastName?: string | null;
  pseudonym?: string | null;
  signedIn?: boolean;
}
