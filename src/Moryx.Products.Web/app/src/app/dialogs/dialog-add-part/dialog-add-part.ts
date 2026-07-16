/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
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

  protected possibleParts = signal<ProductModel[]>([]);
  protected filteredPossibleParts = signal<ProductModel[]>([]);
  protected selectedPart = signal<ProductModel | undefined>(undefined);
  protected searchText = signal('');

  protected TranslationConstants = TranslationConstants;

  constructor() {
    const body = {
      includeDeleted: false,
      revisionFilter: RevisionFilter.All,
      selector: Selector.Direct,
      typeName: this.data.type,
    } as ProductQuery;
    this.productManagementService.getTypes({body: body}).then((products) => {
      let possibleParts = products;
      const currentParts = this.data.parts;
      if (currentParts?.length && !this.data.isCollection) {
        possibleParts = possibleParts.filter((p) => currentParts[0]?.id !== p.id);
      }

      // Todo: Make possible parts a resource signal
      this.possibleParts.set(possibleParts);
      this.filteredPossibleParts.set(possibleParts);
    }).catch((e) => this.snackbarService.handleError(e));
  }

  protected onClose() {
    this.dialogRef.close();
  }

  protected onSelectPart(part: ProductModel) {
    this.selectedPart.set(part);
  }

  protected onSearchTextChanged() {
    this.filteredPossibleParts.set(this.possibleParts().filter((part) =>
      this.partContainsSearchText(part)
    ));
  }

  private partContainsSearchText(part: ProductModel): boolean {
    const name = this.editProductsService.createProductNameWithIdentity(part);
    const indexSearchText = name
      .toLowerCase()
      .indexOf(this.searchText().toLowerCase());
    if (indexSearchText >= 0) {
      return true;
    }
    return false;
  }

  protected createProductNameWithIdentity(product: ProductModel | undefined, shortened: boolean = false, maxLength: number = 40): string {
    return this.editProductsService.createProductNameWithIdentity(product, shortened, maxLength);
  }
}

