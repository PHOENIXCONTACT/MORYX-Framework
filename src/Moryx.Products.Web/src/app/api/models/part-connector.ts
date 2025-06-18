/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework/entry-editor';
import { PartModel } from './part-model';
export interface PartConnector {
  displayName?: null | string;
  isCollection?: boolean;
  name?: null | string;
  parts?: null | Array<PartModel>;
  propertyTemplates?: Entry;
  type?: null | string;
}
