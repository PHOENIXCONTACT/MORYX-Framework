/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from '@angular/core';
import { ShiftInstanceModel } from 'src/app/models/shift-instance-model';
import { ShiftInstanceDialogComponent } from '../shift-instance-dialog/shift-instance-dialog.component';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import AssignmentData from 'src/app/models/assignment-data';
import { CalendarState } from 'src/app/models/calendar-state';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import  moment from 'moment';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { WeekDayToggleButtonComponent } from 'src/app/week-day-toggle-button/week-day-toggle-button.component';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-copy-shift-and-assignment',
  templateUrl: './copy-shift-and-assignment.component.html',
  styleUrl: './copy-shift-and-assignment.component.scss',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatExpansionModule,
    MatDatepickerModule,
    WeekDayToggleButtonComponent,
    MatButtonModule,
    TranslateModule,
    MatDialogModule
]
})
export class CopyShiftAndAssignmentComponent {
  calendarState = signal<CalendarState | undefined>(undefined);
  formData = signal<CopyShiftAndAssignmentData | undefined>(undefined);

  TranslationConstants = TranslationConstants;

  constructor(@Inject(MAT_DIALOG_DATA) public data: CopyShiftAndAssignmentData,
  public dialogRef: MatDialogRef<CopyShiftAndAssignmentComponent>,
  public translate: TranslateService){
    this.calendarState.set(new CalendarState(translate));
    this.formData.set({... this.data});
  }

  onStartDateChanged(shiftInstance: ShiftInstanceModel) {
    const now = moment(shiftInstance.startDate);
    const endDate = now.add(shiftInstance.shiftType.duration-1, 'days').toDate();
    shiftInstance.endDate = endDate;
  }

  deleteItem(shiftInstance: ShiftInstanceModel){
    this.formData.update(form => {
      form!.shiftInstances = form!.shiftInstances.filter(x => x !== shiftInstance);
      return form;
    })
  }

  save(){
    this.dialogRef.close(this.formData());
  }

  cancel(){
    this.dialogRef.close(undefined);
  }

}

export interface CopyShiftAndAssignmentData {
  assignments: Array<AssignmentData>,
  shiftInstances: Array<ShiftInstanceModel>
}

