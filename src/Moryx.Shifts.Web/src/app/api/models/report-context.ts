/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { RestrictionDescription } from '../models/restriction-description';
export interface ReportContext {
  canFinal?: boolean;
  canPartial?: boolean;
  reportedFailure?: number;
  reportedSuccess?: number;
  restrictions?: Array<RestrictionDescription> | null;
  scrapCount?: number;
  successCount?: number;
  unreportedFailure?: number;
  unreportedSuccess?: number;
}

