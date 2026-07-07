/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { OperatorViewModel } from '@app/models/operator-view-model';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { AssignableOperator } from '@app/api/models/assignable-operator';
import { AppStoreService } from '@app/services/app-store.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-add-operator',
  templateUrl: './add-operator.html',
  styleUrl: './add-operator.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    TranslatePipe,
    MatButtonModule
  ]
})
export class AddOperatorDialog {
  private appStoreService = inject(AppStoreService);
  private dialogRef = inject(MatDialogRef<AddOperatorDialog>);

  protected operatorForm = new FormGroup({
    identifier: new FormControl<string>('', [Validators.required]),
    firstName: new FormControl<string>('', [Validators.required]),
    lastName: new FormControl<string>('', [Validators.required]),
    pseudonym: new FormControl<string>('', [Validators.required])
  });
  protected TranslationConstants = TranslationConstants;

  protected getError(control: FormControl<string | null>) {
    return control.hasError('required') ? 'This field is required!' : '';
  }

  protected isValid(control: FormControl<string | null>) {
    return control.valid;
  }

  protected save() {

    if (!this.operatorForm.valid) {
      return;
    }

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

