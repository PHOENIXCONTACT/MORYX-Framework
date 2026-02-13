/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-confirmation-dialog',
    templateUrl: './confirmation-dialog.html',
    styleUrl: './confirmation-dialog.scss',
    standalone: true,
    imports: [
    MatDialogModule,
    TranslateModule
]
})
export class ConfirmationDialog {
  TranslationConstants = TranslationConstants;
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData,
  public dialogRef: MatDialogRef<ConfirmationDialog>) {

  }

  onYesclick(){
    this.data.dialogResult = 'YES';
    this.dialogRef.close(this.data);
  }

  onNoClick(){
    this.data.dialogResult = 'NO';
    this.dialogRef.close(this.data);
  }
}

export interface DialogData {
  dialogMessage: string,
  dialogTitle: string
  dialogResult: 'NO' | 'YES'
}
