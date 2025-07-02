/* tslint:disable */
/* eslint-disable */
import { Entry } from '@moryx/ngx-web-framework';
import { Severity } from './severity';
export interface NotificationModel {
  acknowledged?: null | string;
  created?: string;
  identifier?: string;
  isAcknowledgable?: boolean;
  message?: null | string;
  properties?: Entry;
  sender?: null | string;
  severity?: Severity;
  source?: null | string;
  title?: null | string;
  type?: null | string;
}
