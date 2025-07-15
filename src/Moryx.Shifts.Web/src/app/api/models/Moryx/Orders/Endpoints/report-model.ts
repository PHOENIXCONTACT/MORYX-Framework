/* tslint:disable */
/* eslint-disable */
import { ConfirmationType as MoryxOrdersConfirmationType } from '../../../../models/Moryx/Orders/confirmation-type';
export interface ReportModel {
  comment?: string | null;
  confirmationType?: MoryxOrdersConfirmationType;
  failureCount?: number;
  successCount?: number;
  userIdentifier?: string | null;
}
