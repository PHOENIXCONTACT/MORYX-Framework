/* tslint:disable */
/* eslint-disable */
import { ActivityClassification } from './activity-classification';
import { ActivityResourceModel } from './activity-resource-model';
import { TracingModel } from './tracing-model';
export interface ProcessActivityModel {
  classification?: ActivityClassification;
  id?: number;
  instanceId?: null | number;
  isCompleted?: boolean;
  possibleResources?: null | Array<ActivityResourceModel>;
  requiredCapabilities?: null | string;
  resource?: ActivityResourceModel;
  result?: null | number;
  resultName?: null | string;
  state?: null | string;
  tracing?: TracingModel;
  type?: null | string;
}
