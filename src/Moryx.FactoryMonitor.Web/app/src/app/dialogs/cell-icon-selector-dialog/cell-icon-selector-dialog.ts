/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from '@angular/core';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from '@angular/material/dialog';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatInputModule } from '@angular/material/input';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-cell-icon-selector-dialog',
  templateUrl: './cell-icon-selector-dialog.html',
  styleUrls: ['./cell-icon-selector-dialog.scss'],
  imports: [
    MatDialogModule,
    CdkScrollable,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule
],
  standalone: true,
})
export class CellIconUploaderDialog {
  iconControl = new FormControl<string | null>(null, Validators.required);
  TranslationConstants = TranslationConstants;
  cellName = signal<string | undefined>(undefined);
  matcher = new MyErrorStateMatcher();

  constructor(
    public dialogRef: MatDialogRef<CellIconUploaderDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { cellName: string; iconName: string },
    public translate: TranslateService
  ) {
    this.cellName.set(data.cellName);
    this.iconControl.patchValue(this.data.iconName);
  }

  saveIcon() {
    this.dialogRef.close(this.iconControl.value);
  }

  get canSave() {
    return this.data.iconName != this.iconControl.value && this.iconControl.value != null && this.iconControl.value != '';
  }
}

