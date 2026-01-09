/* tslint:disable */
/* eslint-disable */
import { ConfirmationType } from '../models/confirmation-type';
export interface ReportModel {
  comment?: string | null;
  confirmationType?: ConfirmationType;
  failureCount?: number;
  successCount?: number;
  userIdentifier?: string | null;
}
