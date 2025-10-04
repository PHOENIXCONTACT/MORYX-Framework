/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
export interface TracingModel {
  completed?: string | null;
  errorCode?: number;
  properties?: Entry;
  started?: string | null;
  text?: string | null;
  type?: string | null;
}
