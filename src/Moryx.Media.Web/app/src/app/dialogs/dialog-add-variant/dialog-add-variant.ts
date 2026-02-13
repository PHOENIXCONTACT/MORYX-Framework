/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { MatFormField, MatLabel, MatSuffix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'app-dialog-add-variant',
    templateUrl: './dialog-add-variant.html',
    styleUrls: ['./dialog-add-variant.scss'],
    imports: [
      MatDialogTitle, MatDialogContent,
      MatFormField, MatLabel, MatInput,
      FormsModule, MatIconButton, MatSuffix,
      MatIcon, MatDialogActions, MatButton,
      MatDialogClose, TranslateModule]
})
export class DialogAddVariant {
  TranslationConstants = TranslationConstants;
  fileName = signal<string | undefined>(undefined);
  resultData = signal<AddVariantResultData>({} as AddVariantResultData);
  selectedFileLoaded = signal<boolean>(false);

  constructor(
    public dialogRef: MatDialogRef<DialogAddVariant>,
    public translate: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: string
  ) {
    this.resultData.update(item => {
      item.contentId = data;
      return item;
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    this.selectedFileLoaded.update(_ => false);
    if (file) {
      this.fileName .update(_ => file.name);
      this.resultData.update( item => {
        item.file = file;
        return item;
      })
      this.selectedFileLoaded.update(_ => true);
    }
  }
}

export interface AddVariantResultData {
  contentId: string;
  variantName: string;
  file: File;
}

