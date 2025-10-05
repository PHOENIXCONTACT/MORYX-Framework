/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { NodeConnectionPoint } from './node-connection-point';
import { WorkplanNodeClassification } from './workplan-node-classification';
export interface WorkplanNodeModel {
  classification?: WorkplanNodeClassification;
  displayName?: null | string;
  id?: number;
  inputs?: null | Array<NodeConnectionPoint>;
  name?: null | string;
  outputs?: null | Array<NodeConnectionPoint>;
  positionLeft?: number;
  positionTop?: number;
  properties?: Entry;
  subworkplanId?: number;
  type?: null | string;
}
