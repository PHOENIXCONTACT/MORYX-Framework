/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { PartConnector, ProductModel, RevisionFilter, Selector } from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { EditProductsService } from '../../services/edit-products.service';
import { CommonModule } from '@angular/common';
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
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class DialogAddPartComponent {
  possibleParts = signal<ProductModel[]>([]);
  filteredPossibleParts = signal<ProductModel[]>([]);
  selectedPart = signal<ProductModel | undefined>(undefined);
  searchText = signal('');

  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<DialogAddPartComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PartConnector,
    managementService: ProductManagementService,
    public editService: EditProductsService,
    public translate: TranslateService,
    private snackbarService: SnackbarService
  ) {
    const body = {
      includeDeleted: false,
      revisionFilter: RevisionFilter.All,
      selector: Selector.Direct,
      type: data?.type,
    };
    managementService.getTypes({body: body}).subscribe({
      next: (products) => {
        let possibleParts = [] as ProductModel[];
        if (data && data.parts?.length && !data.isCollection) {
          possibleParts = products.filter(
            (p) => !data.parts?.some((s) => s.product?.id === p.id)
          );
        } else {
          possibleParts = products;
        }

        this.possibleParts.update(_ => possibleParts);
        this.filteredPossibleParts.update(_ => possibleParts);
      },
      error: async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e),
    });
  }

  onClose() {
    this.dialogRef.close();
  }

  onSelectPart(part: ProductModel) {
    this.selectedPart.update(_ => part);
  }

  onSearchTextChanged() {
    this.filteredPossibleParts.update(_ => this.possibleParts().filter((part) =>
      this.partContainsSearchText(part)
    ));
  }

  partContainsSearchText(part: ProductModel): boolean {
    const name = this.editService.createProductNameWithIdentity(part);
    const indexSearchText = name
      .toLowerCase()
      .indexOf(this.searchText().toLowerCase());
    if (indexSearchText >= 0) return true;
    return false;
  }
}

