/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { InstructionItemModel } from './instruction-item-model';
import { InstructionResultModel } from './instruction-result-model';
import { InstructionType } from './instruction-type';
export interface InstructionModel {
  id?: number;
  items?: null | Array<InstructionItemModel>;
  inputs?: null | Entry;
  possibleResults?: null | Array<string>;
  results?: null | Array<InstructionResultModel>;
  sender?: null | string;
  type?: InstructionType;
}
