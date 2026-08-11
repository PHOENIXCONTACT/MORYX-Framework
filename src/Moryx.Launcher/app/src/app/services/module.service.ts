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
/** Holds the registered modules and provides filtered/sorted views and active-route detection. */
export class ModuleService {
  /** All registered modules loaded from the server. */
  private readonly _modules = signal<ModuleItem[]>([]);
  readonly modules = this._modules.asReadonly();

  /** Called by the app root to populate the module list after loading. */
  setModules(items: ModuleItem[]): void {
    this._modules.set(items);
  }

  /** User-category modules, sorted by sortIndex. */
  userModules = computed(() =>
    this.modules()
      .filter(m => m.category === ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex)
  );

  /** Non-user-category modules, sorted by sortIndex. */
  otherModules = computed(() =>
    this.modules()
      .filter(m => m.category !== ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex)
  );

  /** The route of the best-matching active module based on the current URL. */
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
