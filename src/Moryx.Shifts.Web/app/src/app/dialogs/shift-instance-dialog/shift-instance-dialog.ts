/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import  moment from 'moment';
import { TranslationConstants } from '@app/translation-constants';
import { ShiftInstanceModel } from '@app/models/shift-instance-model';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shift-instance-dialog',
  templateUrl: './shift-instance-dialog.html',
  styleUrl: './shift-instance-dialog.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    ReactiveFormsModule,
    TranslatePipe,
    MatButtonModule
]
})
export class ShiftInstanceDialog {
  protected data = inject<ShiftInstanceModel>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<ShiftInstanceDialog>);

  protected form = new FormGroup({
    startDate: new FormControl<Date>(new Date()),
    endDate: new FormControl<Date>(new Date())
  });
  protected TranslationConstants = TranslationConstants;

  constructor() {
    this.form.patchValue({
      startDate: this.data.startDate,
      endDate: this.data.endDate
    });
  }

  protected submit() {
    if (!this.form.valid) {
      return;
    }
    if (this.form.value.startDate) {
      this.data.startDate = this.form.value.startDate;
    }
    if (this.form.value.endDate) {
      this.data.endDate = this.form.value.endDate;
    }
    this.dialogRef.close(this.data);
  }

  protected onStartDateChanged(event: Date | null) {
    const now = moment(this.form.value.startDate);
    const endDate = now.add(this.data.shiftType.duration-1, 'days').toDate();
    this.form.controls.endDate.setValue(endDate);
  }
}

