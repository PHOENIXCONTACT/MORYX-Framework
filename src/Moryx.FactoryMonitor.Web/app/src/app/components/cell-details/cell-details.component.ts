/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Input, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CellImageDialogComponent } from 'src/app/dialogs/cell-image-dialog/cell-image-dialog.component';
import { DetailsConfigurationDialogComponent } from 'src/app/dialogs/details-configuration-dialog/details-configuration-dialog.component';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CellSettingsModel } from 'src/app/api/models/cell-settings-model';
import { TranslateService, TranslatePipe, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import Cell from 'src/app/models/cell';
import { NgIf, NgFor, KeyValuePipe, CommonModule } from '@angular/common';
import { MatButtonModule, MatIconButton } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { TrafficLightComponent } from '../traffic-light/traffic-light.component';
import { DetailsItemComponent } from '../details-item/details-item.component';

@Component({
  selector: 'app-cell-details',
  templateUrl: './cell-details.component.html',
  styleUrls: ['./cell-details.component.scss'],
  imports: [
    CommonModule,
    TrafficLightComponent,
    DetailsItemComponent,
    TranslateModule,
    MatButtonModule,
    MatIconModule
  ],
  standalone: true,
})
export class CellDetailsComponent implements OnInit {
  cellDetails = signal<Cell | undefined>(undefined);
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
    this.matDialog.open(DetailsConfigurationDialogComponent, {});
  }

  openCellImageDialog() {
    this.matDialog.open(CellImageDialogComponent, {
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

