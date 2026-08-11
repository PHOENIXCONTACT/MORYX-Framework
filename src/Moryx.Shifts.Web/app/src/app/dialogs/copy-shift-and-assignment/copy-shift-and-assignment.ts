/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { ShiftInstanceModel } from '@app/models/shift-instance-model';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import AssignmentData from '@app/models/assignment-data';
import { CalendarState } from '@app/models/calendar-state';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import moment from 'moment';
import { TranslationConstants } from '@app/translation-constants';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { WeekDayToggleButton } from '@app/week-day-toggle-button/week-day-toggle-button';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-copy-shift-and-assignment',
  templateUrl: './copy-shift-and-assignment.html',
  styleUrl: './copy-shift-and-assignment.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatExpansionModule,
    MatDatepickerModule,
    WeekDayToggleButton,
    MatButtonModule,
    TranslatePipe,
    MatDialogModule
  ],
  providers: [
    provideNativeDateAdapter()
  ],
})
export class CopyShiftAndAssignment {
  private data = inject<CopyShiftAndAssignmentData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<CopyShiftAndAssignment>);
  private translate = inject(TranslateService);

  protected calendarState = signal(new CalendarState(this.translate));
  protected formData = signal<CopyShiftAndAssignmentData>({...this.data});

  protected TranslationConstants = TranslationConstants;

  protected onStartDateChanged(shiftInstance: ShiftInstanceModel) {
    const now = moment(shiftInstance.startDate);
    const endDate = now.add(shiftInstance.shiftType.duration - 1, 'days').toDate();
    shiftInstance.endDate = endDate;
  }

  protected deleteItem(shiftInstance: ShiftInstanceModel) {
    this.formData.update(form => {
      form!.shiftInstances = form!.shiftInstances.filter(x => x !== shiftInstance);
      return form;
    })
  }

  protected save() {
    this.dialogRef.close(this.formData());
  }

  protected cancel() {
    this.dialogRef.close(undefined);
  }
}

export interface CopyShiftAndAssignmentData {
  assignments: Array<AssignmentData>,
  shiftInstances: Array<ShiftInstanceModel>
}

