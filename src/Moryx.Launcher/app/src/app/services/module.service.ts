/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, Injectable, signal } from '@angular/core';
import { ModuleItem } from '../models/module-item';
import { ModuleCategory } from '../models/module-category';

@Injectable({
  providedIn: 'root'
})
export class ModuleService {
  modules = signal<ModuleItem[]>([]);

  userModules = computed(() =>
    this.modules()
      .filter(m => m.category === ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex)
  );

  otherModules = computed(() =>
    this.modules()
      .filter(m => m.category !== ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex)
  );
}
