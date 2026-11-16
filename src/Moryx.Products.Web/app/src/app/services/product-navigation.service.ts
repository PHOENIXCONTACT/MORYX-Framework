/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductNavigationService {
  private readonly _loading = signal(false);
  readonly loading = this._loading.asReadonly();

  private readonly _activeId = signal<number | undefined>(undefined);
  readonly activeId = this._activeId.asReadonly();

  setLoading(loading: boolean) {
    this._loading.set(loading);
  }

  setActiveId(id: number | undefined) {
    this._activeId.set(id);
  }
}
