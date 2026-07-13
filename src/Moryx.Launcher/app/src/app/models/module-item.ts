/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ModuleCategory } from './module-category';

export interface ModuleItem {
  route: string,
  sortIndex: number,
  title: string,
  icon: string,
  description: string,
  category: ModuleCategory,
}
