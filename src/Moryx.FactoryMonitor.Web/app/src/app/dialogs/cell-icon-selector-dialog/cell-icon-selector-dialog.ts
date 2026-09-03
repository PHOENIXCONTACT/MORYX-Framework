/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { MatInputModule } from '@angular/material/input';

import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-cell-icon-selector-dialog',
  templateUrl: './cell-icon-selector-dialog.html',
  styleUrls: ['./cell-icon-selector-dialog.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
    TranslatePipe
]
})
export class CellIconUploaderDialog {
  private dialogRef = inject(MatDialogRef<CellIconUploaderDialog>);
  private data = inject<{ cellName: string; iconName: string }>(MAT_DIALOG_DATA);

  protected iconControl = new FormControl<string | null>(null, Validators.required);
  protected TranslationConstants = TranslationConstants;
  protected cellName = signal<string | undefined>(undefined);
  protected matcher = new MyErrorStateMatcher();

  constructor() {
    this.cellName.set(this.data.cellName);
    this.iconControl.patchValue(this.data.iconName);
  }

  protected saveIcon() {
    this.dialogRef.close(this.iconControl.value);
  }

  protected get canSave() {
    return this.data.iconName != this.iconControl.value && this.iconControl.value != null && this.iconControl.value != '';
  }
}

