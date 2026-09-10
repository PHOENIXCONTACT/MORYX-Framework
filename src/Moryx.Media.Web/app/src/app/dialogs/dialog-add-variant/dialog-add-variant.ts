/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose
} from '@angular/material/dialog';
import { TranslationConstants } from '@app/translation-constants';
import { MatFormField, MatLabel, MatSuffix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-dialog-add-variant',
  templateUrl: './dialog-add-variant.html',
  styleUrls: ['./dialog-add-variant.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogTitle, MatDialogContent,
    MatFormField, MatLabel, MatInput,
    FormsModule, MatIconButton, MatSuffix,
    MatIcon, MatDialogActions, MatButton,
    MatDialogClose, TranslatePipe]
})
export class DialogAddVariant {
  private dialogRef = inject(MatDialogRef<DialogAddVariant>);
  private data = inject<string>(MAT_DIALOG_DATA);

  protected TranslationConstants = TranslationConstants;
  protected fileName = signal<string | undefined>(undefined);
  protected resultData = signal<AddVariantResultData>({} as AddVariantResultData);
  protected selectedFileLoaded = signal<boolean>(false);

  constructor() {
    this.resultData.update(item => {
      item.contentId = this.data;
      return item;
    });
  }

  protected onNoClick(): void {
    this.dialogRef.close();
  }

  protected onFileSelected(event: Event) {
    const file: File = (event.target as HTMLInputElement).files![0];
    this.selectedFileLoaded.set(false);
    if (file) {
      this.fileName.set(file.name);
      this.resultData.update(item => {
        item.file = file;
        return item;
      })
      this.selectedFileLoaded.set(true);
    }
  }
}

export interface AddVariantResultData {
  contentId: string;
  variantName: string;
  file: File;
}

