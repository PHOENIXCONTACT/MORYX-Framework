/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { EditProductsService } from '@app/services/edit-products.service';
import { MatTableModule } from '@angular/material/table';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-product-references',
  templateUrl: './product-references.html',
  styleUrls: ['./product-references.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTableModule,
    TranslateModule,
    EmptyState,
    MatProgressSpinnerModule,
    MatCardModule,
    RouterLink
]
})
export class ProductReferences {
  references = toSignal(inject(EditProductsService).references$, { initialValue: [] });
  TranslationConstants = TranslationConstants;
}
