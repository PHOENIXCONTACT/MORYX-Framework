/* tslint:disable */
/* eslint-disable */
import { ICapabilities } from '../models/i-capabilities';
export interface IOperatorAssignable {
  capabilities?: ICapabilities;
  id?: number;
  name?: string | null;
  requiredSkills?: ICapabilities;
}
