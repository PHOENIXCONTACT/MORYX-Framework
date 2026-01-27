/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
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
  templateUrl: './product-references.component.html',
  styleUrls: ['./product-references.component.scss'],
  standalone: true,
  imports: [
    MatTableModule,
    TranslateModule,
    EmptyState,
    MatProgressSpinnerModule,
    MatCardModule
  ]
})
export class ProductReferencesComponent {

  references = signal<ProductModel[]>([]);
  isLoading = signal(false);
  TranslationConstants = TranslationConstants;

  constructor(
    editService: EditProductsService,
    public translate: TranslateService,
    private router: Router
  ) {
    this.isLoading.update(_ => true);
    editService.references.subscribe((references) => {
      this.references.update(_ => references ?? []);
      this.isLoading.update(_ => false);
    });
  }

  referenceClicked(reference: ProductModel) {
    this.router.navigate(['/details', reference.id, 'properties'])
  }
}

