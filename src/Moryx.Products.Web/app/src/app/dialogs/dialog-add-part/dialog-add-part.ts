/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  inject,
  signal,
  computed,
  resource,
  effect,
  untracked,
  ChangeDetectionStrategy
} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { HttpErrorResponse } from '@angular/common/http';
import { PartConnector, ProductModel, ProductQuery, RevisionFilter, Selector } from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { EditProductsService } from '@app/services/edit-products.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-dialog-add-part',
  templateUrl: './dialog-add-part.html',
  styleUrls: ['./dialog-add-part.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ]
})
export class DialogAddPart {
  private dialogRef = inject(MatDialogRef<DialogAddPart>);
  private data = inject<PartConnector>(MAT_DIALOG_DATA);
  private productManagementService = inject(ProductManagementService);
  private editProductsService = inject(EditProductsService);
  private snackbarService = inject(SnackbarService);

  protected TranslationConstants = TranslationConstants;

  protected selectedPart = signal<ProductModel | undefined>(undefined);
  protected searchText = signal('');

  protected partsResource = resource({
    loader: async () => {
      const body: ProductQuery = {
        includeDeleted: false,
        revisionFilter: RevisionFilter.All,
        selector: Selector.Direct,
        typeName: this.data.type,
      };

      let possibleParts = await this.productManagementService.getTypes({body});
      const currentParts = this.data.parts;
      if (currentParts?.length && !this.data.isCollection) {
        possibleParts = possibleParts.filter((p) => currentParts[0]?.id !== p.id);
      }

      return possibleParts;
    }
  });

  protected possibleParts = computed(() => this.partsResource.value() ?? []);
  protected filteredPossibleParts = computed(() =>
    this.possibleParts().filter(part => this.partContainsSearchText(part))
  );

  constructor() {
    effect(() => {
      const error = this.partsResource.error();
      if (error) {
        untracked(() => {
          this.snackbarService.handleError(error as HttpErrorResponse)
        });
      }
    });
  }

  protected onClose() {
    this.dialogRef.close();
  }

  protected onSelectPart(part: ProductModel) {
    this.selectedPart.set(part);
  }

  private partContainsSearchText(part: ProductModel): boolean {
    const name = this.editProductsService.createProductNameWithIdentity(part);
    const indexSearchText = name.toLowerCase().indexOf(this.searchText().toLowerCase());

    return indexSearchText >= 0;
  }

  protected createProductNameWithIdentity(product: ProductModel | undefined): string {
    return this.editProductsService.createProductNameWithIdentity(product);
  }
}

