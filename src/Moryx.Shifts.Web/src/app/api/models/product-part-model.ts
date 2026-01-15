/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { PartClassification } from '../models/part-classification';
import { StagingIndicator } from '../models/staging-indicator';
export interface ProductPartModel {
  classification?: PartClassification;
  id?: number;
  identifier?: string | null;
  name?: string | null;
  quantity?: number;
  stagingIndicator?: StagingIndicator;
  unit?: string | null;
}

