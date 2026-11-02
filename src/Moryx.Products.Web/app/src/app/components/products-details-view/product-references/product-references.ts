/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { EditProductsService } from '@app/services/edit-products.service';
import { ProductManagementService } from '@api/services';
import { ProductQuery, RevisionFilter, Selector } from '@api/models';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-references',
  templateUrl: './product-references.html',
  styleUrls: ['./product-references.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTableModule,
    TranslatePipe,
    EmptyState,
    MatProgressSpinnerModule,
    MatCardModule,
    RouterLink
]
})
export class ProductReferences implements OnInit {
  private editProductsService = inject(EditProductsService);
  private productManagementService = inject(ProductManagementService);
  private snackbarService = inject(SnackbarService);

  protected loading = signal(true);
  protected references = this.editProductsService.references;
  protected TranslationConstants = TranslationConstants;

  ngOnInit() {
    this.loadReferences();
  }

  private async loadReferences() {
    const product = this.editProductsService.currentProduct();
    if (!product) {
      this.loading.set(false);
      return;
    }

    const body: ProductQuery = {
      includeDeleted: false,
      identifier: product.identifier,
      revision: product.revision,
      revisionFilter: RevisionFilter.Specific,
      selector: Selector.Parent,
    };

    try {
      const references = await this.productManagementService.getTypes({ body });
      this.editProductsService.setReferences(references);
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
    } finally {
      this.loading.set(false);
    }
  }
}
