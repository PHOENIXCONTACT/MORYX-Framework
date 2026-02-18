/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { DuplicateProductInfos } from 'src/app/models/DuplicateProductInfos';
import { ProductModel } from '../../api/models';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-duplicate-product',
  templateUrl: './dialog-duplicate-product.html',
  styleUrls: ['./dialog-duplicate-product.scss'],
  imports: [
    MatFormFieldModule,
    FormsModule,
    TranslateModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule
  ]
})
export class DialogDuplicateProductComponent {
  private dialogRef = inject(MatDialogRef<DialogDuplicateProductComponent>);
  private data = inject<ProductModel>(MAT_DIALOG_DATA);

  productToDuplicate = signal<ProductModel | undefined>(undefined);
  duplicateInfos = signal<DuplicateProductInfos | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  constructor() {
    this.productToDuplicate.update(_ => this.data);
    this.duplicateInfos.update(_ => {
      return {product: this.data} as DuplicateProductInfos
    });
  }

  onClose() {
    this.dialogRef.close();
  }
}

