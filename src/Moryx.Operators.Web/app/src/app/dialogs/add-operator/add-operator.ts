/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { OperatorViewModel } from 'src/app/models/operator-view-model';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { AssignableOperator } from 'src/app/api/models/assignable-operator';
import { AppStoreService } from 'src/app/services/app-store.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-add-operator',
  templateUrl: './add-operator.html',
  styleUrl: './add-operator.scss',
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    TranslateModule,
    MatButtonModule
  ]
})
export class AddOperatorDialog {
  private appStoreService = inject(AppStoreService);
  private dialogRef = inject(MatDialogRef<AddOperatorDialog>);

  operatorForm = new FormGroup({
    identifier: new FormControl<string>('', [Validators.required]),
    firstName: new FormControl<string>('', [Validators.required]),
    lastName: new FormControl<string>('', [Validators.required]),
    pseudonym: new FormControl<string>('', [Validators.required])
  });
  TranslationConstants = TranslationConstants;

  getError(control: FormControl<string | null>) {
    return control.hasError('required') ? 'This field is required!' : '';
  }

  isValid(control: FormControl<string | null>) {
    return control.valid;
  }

  save() {

    if (!this.operatorForm.valid) return;

    const operator = <AssignableOperator>{
      firstName: this.operatorForm.value.firstName,
      lastName: this.operatorForm.value.lastName,
      identifier: this.operatorForm.value.identifier,
      pseudonym: this.operatorForm.value.pseudonym,
      signedIn: false
    };

    const data = new OperatorViewModel(operator);

    this.appStoreService.addOperator(data);

    this.dialogRef.close(data);
  }
}

