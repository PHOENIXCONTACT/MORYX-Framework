/* tslint:disable */
/* eslint-disable */
import { LogLevel } from '../models/log-level';
export interface OperationLogMessageModel {
  exception?: string | null;
  logLevel?: LogLevel;
  message?: string | null;
  timeStamp?: string;
}
