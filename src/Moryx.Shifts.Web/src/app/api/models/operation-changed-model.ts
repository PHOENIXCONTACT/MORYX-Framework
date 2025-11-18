/* tslint:disable */
/* eslint-disable */
import { OperationAdvicedModel } from '../models/operation-adviced-model';
import { OperationReportedModel } from '../models/operation-reported-model';
import { OperationStartedModel } from '../models/operation-started-model';
export interface OperationChangedModel {
  advicedModel?: OperationAdvicedModel;
  reportedModel?: OperationReportedModel;
  startedModel?: OperationStartedModel;
}
