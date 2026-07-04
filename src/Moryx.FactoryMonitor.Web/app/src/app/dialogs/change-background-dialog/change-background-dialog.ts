/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { ChangeBackgroundService } from '@app/services/change-background.service';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { TranslatePipe } from '@ngx-translate/core';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-change-background-dialog',
  templateUrl: './change-background-dialog.html',
  styleUrls: ['./change-background-dialog.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    TranslatePipe,
  ]
})
export class ChangeBackgroundDialog {
  protected backgroundUrlFormControl = new FormControl<string>('', Validators.required);
  protected TranslationConstants = TranslationConstants;
  private dialogRef = inject(MatDialogRef<ChangeBackgroundDialog>);
  protected backgroundChangeService = inject(ChangeBackgroundService);
  protected matcher = new MyErrorStateMatcher();

  //save the link
  protected onSave() {
    this.backgroundChangeService.changeBackground(this.backgroundUrlFormControl.value ?? '');
    this.dialogRef.close();
  }
}

