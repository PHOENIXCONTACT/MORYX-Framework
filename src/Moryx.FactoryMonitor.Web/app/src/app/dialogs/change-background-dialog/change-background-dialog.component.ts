/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MatDialogModule,
} from '@angular/material/dialog';
import { ChangeBackgroundService } from 'src/app/services/change-background.service';
import { Observable } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateService, TranslatePipe, TranslateModule } from '@ngx-translate/core';
import { MyErrorStateMatcher } from '../MyErrorStateMatcher';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { AsyncPipe, CommonModule } from '@angular/common';
import { MatFormField, MatLabel, MatInput, MatError, MatInputModule } from '@angular/material/input';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-change-background-dialog',
  templateUrl: './change-background-dialog.component.html',
  styleUrls: ['./change-background-dialog.component.scss'],
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
export class ChangeBackgroundDialogComponent {
  backgroundUrlFormControl = new FormControl<string>('', Validators.required);
  TranslationConstants = TranslationConstants;
  dialogRef = inject(MatDialogRef<ChangeBackgroundDialogComponent>);
  backgroundChangeService = inject(ChangeBackgroundService);
  translate = inject(TranslateService);
  matcher = new MyErrorStateMatcher();

  //save the link
  onSave() {
    this.backgroundChangeService.changeBackground(this.backgroundUrlFormControl.value ?? '');
    this.dialogRef.close();
  }
}

