/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FactoryMonitorService } from '../api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { FactorySelectionService } from './factory-selection.service';
import { environment } from 'src/environments/environment';
import { FactoryStateModel } from '../api/models/factory-state-model';

@Injectable({
  providedIn: 'root'
})
export class ChangeBackgroundService {
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);
  private factorySelectionService = inject(FactorySelectionService);

  defaultUrl = environment.rootUrl + '/background.PNG';
  private _factory?: number;
  private _backgroundChanged = new BehaviorSubject<string>(this.defaultUrl);
  public backgroundChanged$ = this._backgroundChanged.asObservable();
  private _canSaveBackground = new BehaviorSubject<boolean>(false);
  public canSaveBackground$ = this._canSaveBackground.asObservable();

  constructor() {
    this.factorySelectionService.factorySelected$.subscribe({
      next: factorySelected => {
        this._factory = factorySelected;
        this._canSaveBackground.next(true);
      }
    });
    this.factoryMonitorService.initialFactoryState().subscribe({
      next: (fsm: FactoryStateModel) => {
        this._backgroundChanged.next(fsm.backgroundURL ?? this.defaultUrl);
      }
    });
  }

  public changeBackground(url: string) {
    if (!url || !this._factory) return;

    this.factoryMonitorService
      .changeBackground({
        resourceId: this._factory,
        url: url
      })
      .subscribe({
        next: () => {
          this._backgroundChanged.next(url);
        },
        error: () => this.snackbarService.showError('An error occured while saving the background URL')
      });
  }

  public changeLocalBackground(url: string) {
    this._backgroundChanged.next(url);
  }
}

