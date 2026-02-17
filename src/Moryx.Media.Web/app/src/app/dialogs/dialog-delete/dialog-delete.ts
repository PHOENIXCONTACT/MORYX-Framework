/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose
} from '@angular/material/dialog';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-dialog-delete',
  templateUrl: './dialog-delete.html',
  styleUrls: ['./dialog-delete.scss'],
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatButton,
    MatDialogClose,
    TranslateModule
  ]
})
export class DialogDelete {
  private dialogRef = inject(MatDialogRef<DialogDelete>);
  data = inject<DeleteDialogData>(MAT_DIALOG_DATA);

  TranslationConstants = TranslationConstants;

  constructor() {
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}

export interface DeleteDialogData {
  type: string;
  deleteMessage: string;
}

