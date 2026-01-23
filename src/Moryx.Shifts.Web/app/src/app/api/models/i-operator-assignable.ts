/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { ICapabilities } from '../models/i-capabilities';
export interface IOperatorAssignable {
  capabilities?: ICapabilities;
  id?: number;
  name?: string | null;
  requiredSkills?: ICapabilities;
}

