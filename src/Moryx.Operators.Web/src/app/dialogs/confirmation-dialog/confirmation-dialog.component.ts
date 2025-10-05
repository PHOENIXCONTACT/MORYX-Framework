import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'app-confirmation-dialog',
    templateUrl: './confirmation-dialog.component.html',
    styleUrl: './confirmation-dialog.component.scss',
    standalone: true,
    imports:[
      CommonModule,
      MatDialogModule,
      TranslateModule
    ]
})
export class ConfirmationDialogComponent {
  TranslationConstants = TranslationConstants;
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData,
  public dialogRef: MatDialogRef<ConfirmationDialogComponent>) {

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