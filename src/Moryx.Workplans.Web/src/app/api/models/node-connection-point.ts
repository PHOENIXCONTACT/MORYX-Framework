/* tslint:disable */
/* eslint-disable */
import { NodeConnector } from './node-connector';
export interface NodeConnectionPoint {
  connections?: null | Array<NodeConnector>;
  index?: number;
  name?: null | string;
}
