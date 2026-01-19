/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ProductModel } from '../api/models';

export interface DuplicateProductInfos {
  product?: ProductModel;
  identifier?: string;
  revision?: number;
}

