/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { DuplicateProductInfos } from '@app/models/DuplicateProductInfos';
import { ProductModel } from '@api/models';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-duplicate-product',
  templateUrl: './dialog-duplicate-product.html',
  styleUrls: ['./dialog-duplicate-product.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatFormFieldModule,
    FormsModule,
    TranslatePipe,
    MatDialogModule,
    MatInputModule,
    MatButtonModule
  ]
})
export class DialogDuplicateProduct {
  private dialogRef = inject(MatDialogRef<DialogDuplicateProduct>);
  private data = inject<ProductModel>(MAT_DIALOG_DATA);

  protected productToDuplicate = signal<ProductModel | undefined>(undefined);
  protected duplicateInfos = signal<DuplicateProductInfos | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.productToDuplicate.set(this.data);
    this.duplicateInfos.set({product: this.data} as DuplicateProductInfos);
  }

  protected onClose() {
    this.dialogRef.close();
  }
}

