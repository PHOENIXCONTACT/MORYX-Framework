/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
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
  private dialogRef = inject(MatDialogRef<DialogAddVariant>);
  private data = inject<string>(MAT_DIALOG_DATA);

  TranslationConstants = TranslationConstants;
  fileName = signal<string | undefined>(undefined);
  resultData = signal<AddVariantResultData>({} as AddVariantResultData);
  selectedFileLoaded = signal<boolean>(false);

  constructor() {
    this.resultData.update(item => {
      item.contentId = this.data;
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
      this.fileName.update(_ => file.name);
      this.resultData.update(item => {
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

