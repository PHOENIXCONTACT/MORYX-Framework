/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.html',
  styleUrl: './confirmation-dialog.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    TranslatePipe
  ]
})
export class ConfirmationDialog {
  protected data = inject<DialogData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<ConfirmationDialog>);

  protected TranslationConstants = TranslationConstants;

  protected onYesclick() {
    this.data.dialogResult = 'YES';
    this.dialogRef.close(this.data);
  }

  protected onNoClick() {
    this.data.dialogResult = 'NO';
    this.dialogRef.close(this.data);
  }
}

export interface DialogData {
  dialogMessage: string,
  dialogTitle: string
  dialogResult: 'NO' | 'YES'
}
