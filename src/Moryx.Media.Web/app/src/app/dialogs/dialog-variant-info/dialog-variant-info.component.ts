/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatLine } from '@angular/material/core';
import { CdkCopyToClipboard } from '@angular/cdk/clipboard';
import { MatButton } from '@angular/material/button';
import { DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-dialog-variant-info',
    templateUrl: './dialog-variant-info.component.html',
    styleUrls: ['./dialog-variant-info.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatLine, CdkCopyToClipboard, MatDialogActions, MatButton, MatDialogClose, DecimalPipe, TranslateModule]
})
export class DialogVariantInfoComponent {
  TranslationConstants = TranslationConstants;

  constructor(
    public translate: TranslateService,
    public dialogRef: MatDialogRef<DialogVariantInfoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: VariantInfoDialogData
  ) {}

  onNoClick(): void {
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

