/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { DuplicateProductInfos } from '@app/models/DuplicateProductInfos';
import { ProductModel } from '@api/models';
import { EditProductsService } from '@app/services/edit-products.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-create-revision',
  templateUrl: './dialog-create-revision.html',
  styleUrls: ['./dialog-create-revision.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe,
    MatFormFieldModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ]
})
export class DialogCreateRevision {
  private dialogRef = inject(MatDialogRef<DialogCreateRevision>);
  private data = inject<ProductModel>(MAT_DIALOG_DATA);
  private editService = inject(EditProductsService);

  protected product = signal<ProductModel | undefined>(undefined);
  protected revision = signal<number | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.product.set(this.data);
  }

  protected onClose() {
    this.dialogRef.close();
  }

  protected onCreate() {
    if (this.revision === undefined) {
      return;
    }

    this.dialogRef.close();
    const infos = <DuplicateProductInfos>{};
    infos.product = this.product();
    infos.identifier = this.product()?.identifier ?? '';
    infos.revision = this.revision();
    this.editService.onDuplicate(infos);
  }


  protected createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editService.createProductIdentity(identifier, revision);
  }
}

