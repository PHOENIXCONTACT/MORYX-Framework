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

  activeRoute = computed(() => {
    const pathname = window.location.pathname;
    let best: string | null = null;
    for (const module of this.modules()) {
      const route = module.route;
      const idx = pathname.indexOf(route);
      if (idx < 0) {
        continue;
      }
      const afterChar = pathname[idx + route.length];
      if (afterChar === undefined || afterChar === '/' || afterChar === '?') {
        if (!best || route.length > best.length) {
          best = route;
        }
      }
    }
    return best;
  });
}
