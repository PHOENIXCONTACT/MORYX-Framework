/* tslint:disable */
/* eslint-disable */
import { LogLevel } from './log-level';
export interface OperationLogMessageModel {
  exception?: null | string;
  logLevel?: LogLevel;
  message?: null | string;
  timeStamp?: string;
}
