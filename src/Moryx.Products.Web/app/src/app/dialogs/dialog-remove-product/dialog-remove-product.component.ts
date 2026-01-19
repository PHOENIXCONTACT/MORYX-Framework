/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import {
  ProductModel,
  ProductQuery,
  RevisionFilter,
  Selector,
} from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { CommonModule } from '@angular/common';
import { MatListItem, MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-dialog-remove-product',
    templateUrl: './dialog-remove-product.component.html',
    styleUrls: ['./dialog-remove-product.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      TranslateModule,
      MatListModule,
      MatDialogModule,
      MatButtonModule
    ]
})
export class DialogRemoveProductComponent {
  productToBeRemoved = signal<ProductModel | undefined>(undefined);
  productsWhichContainProduct = signal<ProductModel[]>([]);
  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<DialogRemoveProductComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductModel,
    private managementService: ProductManagementService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {
    this.productToBeRemoved.update(_=> data);
    const body = <ProductQuery>{
      includeDeleted: false,
      identifier: this.productToBeRemoved()?.identifier,
      revision: this.productToBeRemoved()?.revision,
      revisionFilter: RevisionFilter.Specific,
      selector: Selector.Parent,
    };
    this.managementService.getTypes({ body: body }).subscribe({
      next: (references) => {
        this.productsWhichContainProduct.update(_=> references);
      },
      error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e)
    });
  }

  onClose() {
    this.dialogRef.close();
  }
}

