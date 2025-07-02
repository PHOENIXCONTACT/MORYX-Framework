/* tslint:disable */
/* eslint-disable */
import { ConfirmationType } from './confirmation-type';
export interface ReportModel {
  comment?: null | string;
  confirmationType?: ConfirmationType;
  failureCount?: number;
  successCount?: number;
  userIdentifier?: null | string;
}
