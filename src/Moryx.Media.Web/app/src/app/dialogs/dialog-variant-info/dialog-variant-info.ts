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
import { CdkCopyToClipboard } from '@angular/cdk/clipboard';
import { MatButton } from '@angular/material/button';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-dialog-variant-info',
  templateUrl: './dialog-variant-info.html',
  styleUrls: ['./dialog-variant-info.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogTitle,
    MatDialogContent,
    CdkCopyToClipboard,
    MatDialogActions,
    MatButton,
    MatDialogClose,
    DecimalPipe,
    TranslatePipe
  ]
})
export class DialogVariantInfo {
  private dialogRef = inject(MatDialogRef<DialogVariantInfo>);
  protected data = inject<VariantInfoDialogData>(MAT_DIALOG_DATA);

  protected TranslationConstants = TranslationConstants;

  constructor() {
  }

  protected onNoClick(): void {
    this.dialogRef.close();
  }
}

export interface VariantInfoDialogData {
  name: string;
  contentName: string;
  contentId: string;
  creationDate: Date | string;
  size: number;
  url: string;
}

