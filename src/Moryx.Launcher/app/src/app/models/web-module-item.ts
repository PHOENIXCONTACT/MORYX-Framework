/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ModuleItem } from './module-item';

export interface WebModuleItem extends ModuleItem {
  eventStream?: string
}
