/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { LogLevel } from '../models/log-level';
export interface OperationLogMessageModel {
  exception?: string | null;
  logLevel?: LogLevel;
  message?: string | null;
  timeStamp?: string;
}

