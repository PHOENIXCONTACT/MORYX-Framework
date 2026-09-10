/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import {
  MatDialogRef,
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from '@angular/material/dialog';
import { CellIconUploaderDialog } from '../cell-icon-selector-dialog/cell-icon-selector-dialog';
import { environment } from '../../../environments/environment';
import { CellSettingsService } from '@app/services/cell-settings.service';
import { CellSettingsModel } from '@app/api/models/cell-settings-model';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslationConstants } from '@app/translation-constants';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-cell-image-dialog',
  templateUrl: './cell-image-dialog.html',
  styleUrls: ['./cell-image-dialog.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    TranslatePipe
  ]
})
export class CellImageDialog {
  private cellImageDialogRef = inject(MatDialogRef<CellImageDialog>);
  private matDialog = inject(MatDialog);
  protected data = inject<{ name: string; cellId: number; cellSettings: CellSettingsModel }>(MAT_DIALOG_DATA);
  private cellSettingsService = inject(CellSettingsService);

  protected cellSettings = signal<CellSettingsModel | undefined>(undefined);
  protected name!: string;
  protected imageControl = new FormControl<string | null>(null, Validators.required);
  protected TranslationConstants = TranslationConstants;
  protected matcher = new MyErrorStateMatcher();

  constructor() {
    this.cellSettings.set(this.data.cellSettings);
    this.name = this.data.name;
    //checks if there is an image url
    if (this.cellSettings()?.image) {
      this.imageControl.patchValue(this.cellSettings()?.image ?? null);
    }
  }

  protected openCellIconUploader() {
    const cellIconDialog = this.matDialog.open(CellIconUploaderDialog, {
      data: {
        cellName: this.name,
        iconName: this.cellSettings()?.icon,
      }
    });

    cellIconDialog.afterClosed().subscribe(result => {
      //set the icon from the user input dialog
      if (result) {
        this.cellSettings.set(result);
      }
    });
  }

  protected saveCellSettings() {
    this.cellSettingsService.changeCellSettings(this.data.cellId, this.cellSettings()!);
    this.matDialog.closeAll();
  }

  protected urlChanged() {
    //when the input/url value changes update the image displayed
    this.cellSettings.update(cell => {
      cell!.image = this.imageControl.value ?? environment.assets + 'assets/Bedienstation.png';
      return cell;
    })
  }
}

