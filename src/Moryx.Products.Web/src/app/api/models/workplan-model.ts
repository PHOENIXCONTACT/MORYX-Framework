/* tslint:disable */
/* eslint-disable */
import { WorkplanState } from './workplan-state';
export interface WorkplanModel {
  id?: number;
  name?: null | string;
  state?: WorkplanState;
  version?: number;
}
