/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ProductModel, RevisionFilter } from '../../api/models';
import { ProductManagementService } from '../../api/services';
import { EditProductsService } from '../../services/edit-products.service';
import { CommonModule } from '@angular/common';
import { MatActionList, MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-show-revisions',
  templateUrl: './dialog-show-revisions.html',
  styleUrls: ['./dialog-show-revisions.scss'],
  imports: [
    CommonModule,
    TranslateModule,
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
  private route = inject(ActivatedRoute);
  private snackbarService = inject(SnackbarService);

  revisions = signal<ProductModel[]>([]);
  product = signal<ProductModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  constructor() {
    this.product.update(_ => this.data);
  }

  ngOnInit(): void {
    const body = {
      identifier: this.product()?.identifier,
      revisionFilter: RevisionFilter.All,
    };
    this.managementService.getTypes({body: body}).subscribe({
      next: (products) => {
        if (products !== null) this.revisions.update(_ => products);
      },
      error: async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e)
    });
  }

  onClose() {
    this.dialogRef.close();
  }

  onOpen(product: ProductModel) {
    this.dialogRef.close();
    const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
    if (regexSpecificRecipe.test(this.router.url)) {
      this.router.navigate(['../../'], {relativeTo: this.route}).then(() => {
        this.router
          .navigate([`/details/${product.id}`])
          .then(() => this.editService.loadProductById(product.id ?? 0));
      });
    } else {
      this.router.navigate([`/details/${product.id}`]).then(() => this.editService.loadProduct());
    }
  }

  createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editService.createProductIdentity(identifier, revision);
  }
}

