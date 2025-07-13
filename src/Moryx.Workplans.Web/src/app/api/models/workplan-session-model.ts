/* tslint:disable */
/* eslint-disable */
import { WorkplanNodeModel } from './workplan-node-model';
import { WorkplanState } from './workplan-state';
export interface WorkplanSessionModel {
  name?: null | string;
  nodes?: null | Array<WorkplanNodeModel>;
  sessionToken?: null | string;
  state?: WorkplanState;
  version?: number;
  workplanId?: number;
}
