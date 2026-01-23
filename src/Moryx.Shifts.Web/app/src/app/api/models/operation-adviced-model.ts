/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { AdviceModel } from '../models/advice-model';
import { OperationModel } from '../models/operation-model';
export interface OperationAdvicedModel {
  advice?: AdviceModel;
  operationModel?: OperationModel;
}

