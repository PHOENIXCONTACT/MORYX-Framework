/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Time } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ShiftTypeModel } from 'src/app/models/shift-type-model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shift-type-dialog',
  templateUrl: './shift-type-dialog.component.html',
  styleUrl: './shift-type-dialog.component.scss',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
    MatButtonModule
]
})
export class ShiftTypeDialogComponent {
  TranslationConstants = TranslationConstants;
  HOURS_REGEX = /^(?:[01][0-9]|2[0-3]):[0-5][0-9](?::[0-5][0-9])?$/
  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    duration: new FormControl<number>(7, [Validators.min(1),Validators.required]),
    startTime: new FormControl<string>('',[Validators.pattern(this.HOURS_REGEX),Validators.required]),
    endTime: new FormControl<string>('',[Validators.pattern(this.HOURS_REGEX),Validators.required]),
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: ShiftTypeModel,
  public dialogRef: MatDialogRef<ShiftTypeDialogComponent>){
    
  }

  submit(){
    if(!this.form.valid) return;

    const startHours = this.form.value.startTime?.split(':')[0] ?? '0';
    const startMinutes = this.form.value.startTime?.split(':')[1] ?? '0';

    const endHours = this.form.value.endTime?.split(':')[0] ?? '0';
    const endMinutes = this.form.value.endTime?.split(':')[1] ?? '0';
    const startTime = <Time>{ hours: Number(startHours), minutes: Number(startMinutes)};
    const endTime = <Time>{ hours: Number(endHours), minutes: Number(endMinutes)};

    this.data.startTime = startTime;
    this.data.endTime = endTime;
    this.data.name = this.form.value.name ?? '';
    this.data.duration = this.form.value.duration ?? 0;
    this.dialogRef.close(this.data);
  } 

}

