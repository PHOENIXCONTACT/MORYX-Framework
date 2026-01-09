/* tslint:disable */
/* eslint-disable */
import { AttendableResourceModel } from '../models/attendable-resource-model';
export interface ExtendedOperatorModel {
  assignedResources?: Array<AttendableResourceModel> | null;
  firstName?: string | null;
  identifier?: string | null;
  lastName?: string | null;
  pseudonym?: string | null;
}
