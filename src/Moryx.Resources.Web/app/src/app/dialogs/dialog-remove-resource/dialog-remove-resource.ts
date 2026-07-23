/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants';
import { ResourceModel } from '@api/models/resource-model';

@Component({
  selector: 'app-dialog-remove-resource',
  templateUrl: './dialog-remove-resource.html',
  styleUrls: ['./dialog-remove-resource.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe,
    MatDialogModule,
    MatButtonModule
  ]
})
export class DialogRemoveResource {
  private dialogRef = inject(MatDialogRef<DialogRemoveResource>);
  private data = inject<ResourceModel>(MAT_DIALOG_DATA);

  protected resourceToBeRemoved = signal<ResourceModel | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.resourceToBeRemoved.set(this.data);
  }

  protected onClose() {
    this.dialogRef.close();
  }
}
