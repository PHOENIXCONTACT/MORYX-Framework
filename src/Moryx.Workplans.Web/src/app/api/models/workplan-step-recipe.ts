/* tslint:disable */
/* eslint-disable */
import { MethodEntry } from './method-entry';
import { WorkplanNodeClassification } from './workplan-node-classification';
export interface WorkplanStepRecipe {
  classification?: WorkplanNodeClassification;
  constructor?: MethodEntry;
  description?: null | string;
  name?: null | string;
  positionLeft?: number;
  positionTop?: number;
  subworkplanId?: number;
  type?: null | string;
}
