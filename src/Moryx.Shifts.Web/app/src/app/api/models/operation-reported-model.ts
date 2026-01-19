/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { OperationModel } from '../models/operation-model';
import { ReportModel } from '../models/report-model';
export interface OperationReportedModel {
  operationModel?: OperationModel;
  report?: ReportModel;
}

