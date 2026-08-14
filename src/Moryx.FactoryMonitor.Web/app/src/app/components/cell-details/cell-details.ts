/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, computed, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CellImageDialog } from '@app/dialogs/cell-image-dialog/cell-image-dialog';
import { CellStoreService } from '@app/services/cell-store.service';
import { CellSettingsModel } from '@api/models/cell-settings-model';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TrafficLight } from '../traffic-light/traffic-light';
import { DetailsItem } from '../details-item/details-item';

@Component({
  selector: 'app-cell-details',
  templateUrl: './cell-details.html',
  styleUrls: ['./cell-details.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    TrafficLight,
    DetailsItem,
    TranslatePipe,
    MatButtonModule,
    MatIconModule
  ]
})
export class CellDetails {
  private matDialog = inject(MatDialog);
  private cellStoreService = inject(CellStoreService);

  protected TranslationConstants = TranslationConstants;

  protected cellDetails = computed(() => {
    const selected = this.cellStoreService.cellSelected();
    const updated = this.cellStoreService.cellUpdated();
    if (selected && updated && selected.id === updated.id) {
      return updated;
    }
    return selected;
  });

  protected openCellImageDialog() {
    this.matDialog.open(CellImageDialog, {
      data: {
        cellId: this.cellDetails()?.id,
        name: this.cellDetails()?.name,
        cellSettings: <CellSettingsModel>{
          image: this.cellDetails()?.image,
          icon: this.cellDetails()?.iconName,
        },
      }
    });
  }
}

