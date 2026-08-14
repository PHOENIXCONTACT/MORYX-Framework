/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { ProductModel, RevisionFilter } from '@api/models';
import { ProductManagementService } from '@api/services';
import { EditProductsService } from '@app/services/edit-products.service';
import { MatActionList, MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-show-revisions',
  templateUrl: './dialog-show-revisions.html',
  styleUrls: ['./dialog-show-revisions.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe,
    MatActionList,
    MatListModule,
    MatDialogModule,
    MatButtonModule,
    MatListModule,
  ]
})
export class DialogShowRevisions implements OnInit {
  private dialogRef = inject(MatDialogRef<DialogShowRevisions>);
  private data = inject<ProductModel>(MAT_DIALOG_DATA);
  private editService = inject(EditProductsService);
  private managementService = inject(ProductManagementService);
  private router = inject(Router);
  private snackbarService = inject(SnackbarService);

  protected revisions = signal<ProductModel[]>([]);
  protected product = signal<ProductModel | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.product.set(this.data);
  }

  ngOnInit(): void {
    const body = {
      identifier: this.product()?.identifier,
      revisionFilter: RevisionFilter.All,
    };
    this.managementService.getTypes({body: body}).then((products) => {
      if (products !== null) {
        this.revisions.set(products);
      }
    }).catch(async (e: HttpErrorResponse) =>
      await this.snackbarService.handleError(e));
  }

  protected onClose() {
    this.dialogRef.close();
  }

  protected onOpen(product: ProductModel) {
    this.dialogRef.close();
    this.router.navigate(['/details', product.id]);
  }

  protected createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editService.createProductIdentity(identifier, revision);
  }
}

