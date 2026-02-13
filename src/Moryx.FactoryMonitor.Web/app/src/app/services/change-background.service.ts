/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
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
  defaultUrl = environment.rootUrl + '/background.PNG';
  private _factory?: FactoryStateModel;
  private _backgroundChanged = new BehaviorSubject<string>(this.defaultUrl);
  public backgroundChanged$ = this._backgroundChanged.asObservable();
  private _canSaveBackground = new BehaviorSubject<boolean>(false);
  public canSaveBackground$ = this._canSaveBackground.asObservable();

  constructor(private factoryMonitorService: FactoryMonitorService,
              private snackbarService: SnackbarService,
              private factorySelectionService: FactorySelectionService) {
    this.factorySelectionService.factorySelected$.subscribe({
      next: factorySelected => {
        this._factory = factorySelected;
        this._factory = factorySelected;
        this.changeLocalBackground(factorySelected?.backgroundURL ?? this.defaultUrl);
        this._canSaveBackground.next(true);
      }
    });
    this.factoryMonitorService.initialFactoryState().subscribe({
      next: () => {
        this._backgroundChanged.next(this._backgroundChanged.getValue());
        this.factoryMonitorService.initialFactoryState().subscribe({
          next: () => {
            this._backgroundChanged.next(this._backgroundChanged.getValue());
          }
        });

      }
    });
  }

  public changeBackground(url: string) {
    if (!url || !this._factory?.id) return;

    this.factoryMonitorService
      .changeBackground({
        resourceId: this._factory.id,
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

