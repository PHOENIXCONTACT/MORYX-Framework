/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CellImageDialog } from 'src/app/dialogs/cell-image-dialog/cell-image-dialog';
import { DetailsConfigurationDialog } from 'src/app/dialogs/details-configuration-dialog/details-configuration-dialog';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CellSettingsModel } from 'src/app/api/models/cell-settings-model';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import CellModel from 'src/app/models/cellModel';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TrafficLight } from '../traffic-light/traffic-light';
import { DetailsItem } from '../details-item/details-item';

@Component({
  selector: 'app-cell-details',
  templateUrl: './cell-details.html',
  styleUrls: ['./cell-details.scss'],
  imports: [
    CommonModule,
    TrafficLight,
    DetailsItem,
    TranslateModule,
    MatButtonModule,
    MatIconModule
  ],
  standalone: true,
})
export class CellDetails implements OnInit {
  cellDetails = signal<CellModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;
  constructor(
    public matDialog: MatDialog,
    private cellStoreService: CellStoreService,
    public translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.cellStoreService.cellSelected$.subscribe({
      next: result => this.cellDetails.set(result),
    });
  }

  openConfigurationDialog() {
    this.matDialog.open(DetailsConfigurationDialog, {});
  }

  openCellImageDialog() {
    this.matDialog.open(CellImageDialog, {
      data: {
        cellId: this.cellDetails()?.id,
        name: this.cellDetails()?.name,
        cellSettings: <CellSettingsModel>{
          image: this.cellDetails()?.image,
          icon: this.cellDetails()?.iconName,
        },
      },
    });
  }
}

