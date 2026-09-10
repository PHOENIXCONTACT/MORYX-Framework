/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { CellState } from '@api/models/cell-state';
import { TranslationConstants } from '@app/translation-constants';
import CellModel from '@app/models/cellModel';
import { CellStoreService } from '@app/services/cell-store.service';

@Component({
  selector: 'app-traffic-light',
  templateUrl: './traffic-light.html',
  styleUrls: ['./traffic-light.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: []
})
export class TrafficLight {
  protected currentState = signal<CellState | undefined | null>(undefined);
  protected currentStateString = signal<string | undefined>(undefined);
  private cellStoreService = inject(CellStoreService);
  private translateService = inject(TranslateService);
  private id: number | undefined;
  protected CellState = CellState;
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const c = this.cellStoreService.cellSelected();
      this.id = c?.id;
      this.updateState(c);
    });

    effect(() => {
      const c = this.cellStoreService.cellUpdated();
      if (c) {
        this.updateState(c);
      }
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([
        TranslationConstants.CELL_DETAILS.IDLE_STATE,
        TranslationConstants.CELL_DETAILS.RUNNING_STATE,
        TranslationConstants.CELL_DETAILS.NOT_READY_TO_WORK_STATE
      ]));
  }

  private async updateState(newCellParameters: CellModel | undefined): Promise<void> {
    if (newCellParameters?.id != this.id) {
      return;
    }

    this.currentStateString.set(await this.getStringState(newCellParameters?.state ?? CellState.Idle));
    this.currentState.set(newCellParameters?.state ?? CellState.Idle);
  }

  private async getStringState(state: CellState) {
    const translations = await this.getTranslations();

    switch (state) {
      case CellState.Idle:
      case CellState.Requested:
        return translations[TranslationConstants.CELL_DETAILS.IDLE_STATE];
      case CellState.NotReadyToWork:
        return translations[TranslationConstants.CELL_DETAILS.NOT_READY_TO_WORK_STATE];
      case CellState.Running:
        return translations[TranslationConstants.CELL_DETAILS.RUNNING_STATE];
      default:
        return translations[TranslationConstants.CELL_DETAILS.IDLE_STATE];
    }
  }
}

