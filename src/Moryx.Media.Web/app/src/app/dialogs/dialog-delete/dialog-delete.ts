/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose
} from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-dialog-delete',
  templateUrl: './dialog-delete.html',
  styleUrls: ['./dialog-delete.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatButton,
    MatDialogClose,
    TranslatePipe
  ]
})
export class DialogDelete {
  private dialogRef = inject(MatDialogRef<DialogDelete>);
  protected data = inject<DeleteDialogData>(MAT_DIALOG_DATA);

  protected TranslationConstants = TranslationConstants;

  constructor() {
  }

  protected onNoClick(): void {
    this.dialogRef.close();
  }
}

export interface DeleteDialogData {
  type: string;
  deleteMessage: string;
}

