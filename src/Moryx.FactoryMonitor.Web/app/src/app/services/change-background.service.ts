/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FactoryMonitorService } from '../api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { FactorySelectionService } from './factory-selection.service';
import { environment } from 'src/environments/environment';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root'
})
export class ChangeBackgroundService {
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);

  private _factory = toSignal(inject(FactorySelectionService).factorySelected$);
  private _backgroundChanged = new BehaviorSubject<string|undefined>(undefined);
  public backgroundChanged$ = this._backgroundChanged.asObservable();
  public canSaveBackground = computed(() => !!this._factory());

  public changeBackground(url: string) {
    if (!url || !this._factory) return;

    this.factoryMonitorService
      .changeBackground({
        resourceId: this._factory(),
        url: url
      })
      .subscribe({
        next: () => {
          this.updateBackground(url);
        },
        error: () => this.snackbarService.showError('An error occured while saving the background URL')
      });
  }

  public updateBackground(url: string | null | undefined) {
    if (!url) {
      this._backgroundChanged.next(undefined);
      return;
    }

    if (!this.isAbsoluteUrl(url)) {
      url = environment.rootUrl + url;
    }
    
    this._backgroundChanged.next(url);
  }
  
  private isAbsoluteUrl(url: string): boolean {
    try {
      return Boolean(new URL(url).origin);
    } catch {
      return false;
    }
  }
}
