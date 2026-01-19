/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { OperationModel } from '../models/operation-model';
export interface OperationStartedModel {
  operationModel?: OperationModel;
  userId?: string | null;
}

