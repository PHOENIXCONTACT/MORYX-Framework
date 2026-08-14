/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './dialog-confirm.html',
  styleUrls: ['./dialog-confirm.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatButtonModule,
    MatDialogModule,
    TranslatePipe
  ]
})
export class ConfirmDialog {
  protected data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
  protected TranslationConstants = TranslationConstants;
}

export interface ConfirmDialogData {
  title: string;
  message: string;
}

