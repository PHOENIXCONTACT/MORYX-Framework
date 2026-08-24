/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { ProductModel, ProductQuery, RevisionFilter, Selector } from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-remove-product',
  templateUrl: './dialog-remove-product.html',
  styleUrls: ['./dialog-remove-product.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    TranslatePipe,
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

  protected productToBeRemoved = signal<ProductModel | undefined>(undefined);
  protected productsWhichContainProduct = signal<ProductModel[]>([]);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.productToBeRemoved.set(this.data);
    const body = <ProductQuery>{
      includeDeleted: false,
      identifier: this.productToBeRemoved()?.identifier,
      revision: this.productToBeRemoved()?.revision,
      revisionFilter: RevisionFilter.Specific,
      selector: Selector.Parent,
    };
    this.productManagementService.getTypes({body: body}).then((references) => {
      this.productsWhichContainProduct.set(references);
    }).catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected onClose() {
    this.dialogRef.close();
  }
}

