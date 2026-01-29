/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { ChangeBackgroundService } from 'src/app/services/change-background.service';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-change-background-dialog',
  templateUrl: './change-background-dialog.html',
  styleUrls: ['./change-background-dialog.scss'],
  imports: [
    MatDialogModule,
    CdkScrollable,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    TranslateModule,
  ],
  standalone: true,
})
export class ChangeBackgroundDialog {
  backgroundUrlFormControl = new FormControl<string>('', Validators.required);
  TranslationConstants = TranslationConstants;
  dialogRef = inject(MatDialogRef<ChangeBackgroundDialog>);
  backgroundChangeService = inject(ChangeBackgroundService);
  translate = inject(TranslateService);
  matcher = new MyErrorStateMatcher();

  //save the link
  onSave() {
    this.backgroundChangeService.changeBackground(this.backgroundUrlFormControl.value ?? '');
    this.dialogRef.close();
  }
}

