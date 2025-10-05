import { Entry } from "@moryx/ngx-web-framework";
import { InstructionResultModel } from './instruction-result-model';
export interface InstructionResponseModel {
  id? : number;
  inputs? : Entry;
  result?: string;
  selectedResult?: InstructionResultModel;
}