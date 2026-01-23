/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { OperatorViewModel } from 'src/app/models/operator-view-model';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { AssignableOperator } from 'src/app/api/models/assignable-operator';
import { AppStoreService } from 'src/app/services/app-store.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-add-operator',
    templateUrl: './add-operator.component.html',
    styleUrl: './add-operator.component.scss',
    standalone: true,
    imports: [
      CommonModule,
      MatDialogModule,
      MatFormFieldModule,
      ReactiveFormsModule,
      MatInputModule,
      TranslateModule,
      MatButtonModule
    ]
})
export class AddOperatorComponentDialog {
  operatorForm = new FormGroup({
    identifier: new FormControl<string>('', [Validators.required]),
    firstName: new FormControl<string>('', [Validators.required]),
    lastName: new FormControl<string>('', [Validators.required]),
    pseudonym: new FormControl<string>('', [Validators.required])  });
  TranslationConstants = TranslationConstants;

  constructor(
    private appStoreService: AppStoreService,
    public dialogRef: MatDialogRef<AddOperatorComponentDialog>){}

  getError(control: FormControl<string | null>){ 
    return control.hasError('required') ? 'This field is required!' : '';
  }

  isValid(control: FormControl<string | null>){
    return control.valid;
  }

  save(){
    
    if(!this.operatorForm.valid) return;

    var operator = <AssignableOperator>{
      firstName: this.operatorForm.value.firstName,
      lastName: this.operatorForm.value.lastName,
      identifier: this.operatorForm.value.identifier,
      pseudonym: this.operatorForm.value.pseudonym,
      signedIn: false
    };

    var data = new OperatorViewModel(operator);
    
      this.appStoreService.addOperator(data);

      this.dialogRef.close(data);  
  }
}

