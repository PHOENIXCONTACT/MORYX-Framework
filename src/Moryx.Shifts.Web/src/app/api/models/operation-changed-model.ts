/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { OperationAdvicedModel } from '../models/operation-adviced-model';
import { OperationReportedModel } from '../models/operation-reported-model';
import { OperationStartedModel } from '../models/operation-started-model';
export interface OperationChangedModel {
  advicedModel?: OperationAdvicedModel;
  reportedModel?: OperationReportedModel;
  startedModel?: OperationStartedModel;
}

