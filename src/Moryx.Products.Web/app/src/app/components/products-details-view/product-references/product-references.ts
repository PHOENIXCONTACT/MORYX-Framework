/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { EditProductsService } from '@app/services/edit-products.service';
import { MatTableModule } from '@angular/material/table';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

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
export class ProductReferences {
  private editProductsService = inject(EditProductsService);

  protected references = this.editProductsService.references;
  protected TranslationConstants = TranslationConstants;
}
