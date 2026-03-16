/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ProductModel } from '../../../api/models';
import { EditProductsService } from '../../../services/edit-products.service';

import { MatTableModule } from '@angular/material/table';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-product-references',
  templateUrl: './product-references.html',
  styleUrls: ['./product-references.scss'],
  imports: [
    MatTableModule,
    TranslateModule,
    EmptyState,
    MatProgressSpinnerModule,
    MatCardModule
  ]
})
export class ProductReferences {
  private editProductsService = inject(EditProductsService);
  private router = inject(Router);

  references = signal<ProductModel[]>([]);
  isLoading = signal(false);
  TranslationConstants = TranslationConstants;

  constructor() {
    // ToDo: Remove loading indicator and use resolver for references
    this.isLoading.update(_ => true);
    this.editProductsService.references$.subscribe((references) => {
      this.references.update(_ => references ?? []);
      this.isLoading.update(_ => false);
    });
  }

  // ToDo: Add clickable indicator to reference list item
  referenceClicked(reference: ProductModel) {
    this.router.navigate(['/details', reference.id, 'properties'])
  }
}

