/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ResourceModel } from 'src/app/api/models/resource-model';

@Component({
  selector: 'app-dialog-remove-resource',
  templateUrl: './dialog-remove-resource.html',
  styleUrls: ['./dialog-remove-resource.scss'],
  imports: [
    TranslateModule,
    MatDialogModule,
    MatButtonModule
  ]
})
export class DialogRemoveResource {
  private dialogRef = inject(MatDialogRef<DialogRemoveResource>);
  private data = inject<ResourceModel>(MAT_DIALOG_DATA);

  resourceToBeRemoved = signal<ResourceModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;

  constructor() {
    this.resourceToBeRemoved.update(_ => this.data);
  }

  onClose() {
    this.dialogRef.close();
  }
}
