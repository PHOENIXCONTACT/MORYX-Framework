/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject, Injectable, signal } from '@angular/core';
import { FactoryMonitorService } from '@api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { FactorySelectionService } from './factory-selection.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChangeBackgroundService {
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);
  private factorySelectionService = inject(FactorySelectionService);

  private readonly _backgroundChanged = signal<string | undefined>(undefined);
  readonly backgroundChanged = this._backgroundChanged.asReadonly();
  public canSaveBackground = computed(() => !!this.factorySelectionService.factorySelected());

  public changeBackground(url: string) {
    if (!url || !this.factorySelectionService.factorySelected()) {
      return;
    }

    this.factoryMonitorService
      .changeBackground({
        resourceId: this.factorySelectionService.factorySelected(),
        url: url
      })
      .then(() => {
        this.updateBackground(url);
      })
      .catch(() => this.snackbarService.showError('An error occured while saving the background URL'));
  }

  public updateBackground(url: string | null | undefined) {
    if (!url) {
      this._backgroundChanged.set(undefined);
      return;
    }

    if (!this.isAbsoluteUrl(url)) {
      url = environment.rootUrl + url;
    }

    this._backgroundChanged.set(url);
  }

  private isAbsoluteUrl(url: string): boolean {
    try {
      return Boolean(new URL(url).origin);
    } catch {
      return false;
    }
  }
}
