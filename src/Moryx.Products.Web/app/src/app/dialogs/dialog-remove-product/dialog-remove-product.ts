/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ProductModel, ProductQuery, RevisionFilter, Selector } from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-remove-product',
  templateUrl: './dialog-remove-product.html',
  styleUrls: ['./dialog-remove-product.scss'],
  imports: [
    CommonModule,
    TranslateModule,
    MatListModule,
    MatDialogModule,
    MatButtonModule
  ]
})
export class DialogRemoveProduct {
  private dialogRef = inject(MatDialogRef<DialogRemoveProduct>);
  private data = inject<ProductModel>(MAT_DIALOG_DATA);
  private productManagementService = inject(ProductManagementService);
  private snackbarService = inject(SnackbarService);

  productToBeRemoved = signal<ProductModel | undefined>(undefined);
  productsWhichContainProduct = signal<ProductModel[]>([]);
  TranslationConstants = TranslationConstants;

  constructor() {
    this.productToBeRemoved.update(_ => this.data);
    const body = <ProductQuery>{
      includeDeleted: false,
      identifier: this.productToBeRemoved()?.identifier,
      revision: this.productToBeRemoved()?.revision,
      revisionFilter: RevisionFilter.Specific,
      selector: Selector.Parent,
    };
    this.productManagementService.getTypes({body: body}).subscribe({
      next: (references) => {
        this.productsWhichContainProduct.update(_ => references);
      },
      error: async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
    });
  }

  onClose() {
    this.dialogRef.close();
  }
}

