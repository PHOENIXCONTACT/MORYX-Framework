/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, OnInit, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { DuplicateProductInfos } from 'src/app/models/DuplicateProductInfos';
import { ProductModel } from '../../api/models';
import { EditProductsService } from '../../services/edit-products.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-create-revision',
  templateUrl: './dialog-create-revision.html',
  styleUrls: ['./dialog-create-revision.scss'],
  standalone: true,
  imports: [
    TranslateModule,
    MatFormFieldModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ]
})
export class DialogCreateRevisionComponent implements OnInit {
  product = signal<ProductModel | undefined>(undefined);
  revision = signal<number | undefined>(undefined);
  TranslationConstants = TranslationConstants;
  Permissions = Permissions;

  constructor(
    public dialogRef: MatDialogRef<DialogCreateRevisionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductModel,
    public editService: EditProductsService,
    public translate: TranslateService,
  ) {
    this.product.update(_ => data);
  }

  onClose() {
    this.dialogRef.close();
  }

  onCreate() {
    if (this.revision === undefined) return;

    this.dialogRef.close();
    let infos = <DuplicateProductInfos>{};
    infos.product = this.product();
    infos.identifier = this.product()?.identifier!;
    infos.revision = this.revision();
    this.editService.onDuplicate(infos);
  }

  ngOnInit(): void {
  }
}

