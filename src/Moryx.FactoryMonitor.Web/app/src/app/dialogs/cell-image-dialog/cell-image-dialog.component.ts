/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from '@angular/core';
import {
  MatDialogRef,
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MatDialogModule,
} from '@angular/material/dialog';
import { CellIconUploaderDialogComponent } from '../cell-icon-selector-dialog/cell-icon-selector-dialog.component';
import { environment } from 'src/environments/environment';
import { CellSettingsService } from 'src/app/services/cell-settings.service';
import { CellSettingsModel } from 'src/app/api/models/cell-settings-model';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { MatIconButton, MatButton, MatButtonModule } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { CdkScrollable } from '@angular/cdk/scrolling';

import { MatFormField, MatLabel, MatInput, MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslatePipe } from '@ngx-translate/core';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-cell-image-dialog',
  templateUrl: './cell-image-dialog.component.html',
  styleUrls: ['./cell-image-dialog.component.scss'],
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    CdkScrollable,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule
],
  standalone: true
})
export class CellImageDialogComponent {
  cellSettings = signal< CellSettingsModel | undefined>(undefined);
  name!: string;
  imageControl = new FormControl<string | null>(null, Validators.required);
  TranslationConstants = TranslationConstants;
  matcher = new MyErrorStateMatcher();

  constructor(
    public cellImageDialogRef: MatDialogRef<CellImageDialogComponent>,
    public matDialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { name: string; cellId: number; cellSettings: CellSettingsModel },
    private cellSettingsService: CellSettingsService
  ) {
    this.cellSettings.set(this.data.cellSettings);
    this.name = data.name;
    //checks if there is an image url
    if (this.cellSettings()?.image) this.imageControl.patchValue(this.cellSettings()?.image!);
  }

  openCellIconUploader() {
    const cellIconDialog = this.matDialog.open(CellIconUploaderDialogComponent, {
      data: {
        cellName: this.name,
        iconName: this.cellSettings()?.icon,
      },
    });

    cellIconDialog.afterClosed().subscribe(result => {
      if (result)
        //set the icon from the user input dialog
        this.cellSettings.set(result);
    });
  }

  saveCellSettings() {
    this.cellSettingsService.changeCellSettings(this.data.cellId, this.cellSettings()!);
    this.matDialog.closeAll();
  }

  urlChanged() {
    //when the input/url value changes update the image displayed
    this.cellSettings.update(cell => {
      cell!.image = this.imageControl.value ?? environment.assets + 'assets/Bedienstation.png';
      return cell;
    })
  }
}

