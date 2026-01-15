/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { ConfirmationType } from '../models/confirmation-type';
export interface ReportModel {
  comment?: string | null;
  confirmationType?: ConfirmationType;
  failureCount?: number;
  successCount?: number;
  userIdentifier?: string | null;
}

