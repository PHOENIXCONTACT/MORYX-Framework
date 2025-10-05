/* tslint:disable */
/* eslint-disable */
import { LogLevel as MicrosoftExtensionsLoggingLogLevel } from '../../../../models/Microsoft/Extensions/Logging/log-level';
export interface OperationLogMessageModel {
  exception?: string | null;
  logLevel?: MicrosoftExtensionsLoggingLogLevel;
  message?: string | null;
  timeStamp?: string;
}
