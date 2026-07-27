/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import AssignmentData from '../../models/assignment-data';
import { OperatorStatus } from '@app/models/operator-model';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  formatDateDigits,
  getDayName,
  getShortDayName,
  isDayInInterval,
  localizedDayName,
  shortDayName,
} from '@app/utils';
import { CalendarDate } from '@app/models/calendar-state';
import moment from 'moment';
import { AppStoreService } from '@app/services/app-store.service';
import { MatSelectModule } from '@angular/material/select';
import { TranslationConstants } from '@app/translation-constants';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { WeekDayToggleButton } from '@app/week-day-toggle-button/week-day-toggle-button';

@Component({
  selector: 'app-week-assignment-dialog',
  templateUrl: './week-assignment-dialog.html',
  styleUrl: './week-assignment-dialog.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    WeekDayToggleButton
  ]
})
export class WeekAssignmentDialog {
  private dialogRef = inject(MatDialogRef<WeekAssignmentDialog>);
  private appStore = inject(AppStoreService);
  protected data = inject<AssignmentData>(MAT_DIALOG_DATA);

  protected operators = signal(this.appStore.operators());
  protected resources = signal(this.appStore.resources());
  protected shiftInstances = this.appStore.shiftInstances;
  protected shiftNumberOfdays = signal(0);

  protected available = OperatorStatus.Available;
  protected onVacation = OperatorStatus.OnVacation;
  protected notAllowed = OperatorStatus.NotAllowed;
  protected notQualified = OperatorStatus.NotQualified;
  protected TranslationConstants = TranslationConstants;
  protected form = new FormGroup({
    operatorId: new FormControl<string>('', [Validators.required]),
    resourceId: new FormControl<number>(0, [Validators.min(0)]),
    priority: new FormControl<number>(0, [Validators.min(0)]),
    notes: new FormControl('')
  });
  protected formatDateDigits = formatDateDigits;
  protected getDayName = getDayName;
  protected getShortDayName = getShortDayName;
  protected isDayInInterval = isDayInInterval;
  protected localizedDayName = localizedDayName;
  protected shortDayName = shortDayName;

  constructor() {
    this.form.patchValue({
      operatorId: this.data.operator?.id ?? '',
      resourceId: this.data.resource?.id ?? -1,
      notes: this.data.notes ?? '',
      priority: this.data.priority ?? 0
    });

    if (this.isResourceSet()) {
      this.resources.set(this.appStore.resources().filter((x) => x.id === this.data.resource?.id));
      this.form.controls.resourceId.disable();
      if (this.data.resource?.id) {
        this.refreshOperators(this.data.resource?.id);
      }
    } else {
      this.operators.update(items => items.filter((x) => x.id === this.data.operator?.id));
      this.form.controls.operatorId.disable();
      if (this.data.operator?.id) {
        this.refreshResources(this.data.operator?.id);
      }
    }

    const foundCurrentShift = this.shiftInstances().find((x) => x.id === this.data.shift.id);
    if (foundCurrentShift) {
      this.shiftNumberOfdays.set(foundCurrentShift.shiftType.duration);
    }
  }

  private refreshOperators(resourceId: number) {
    //skilled operators
    this.appStore
      .getOperatorsBasedOnResource(resourceId)
      .then((skilledOperators) => {
        this.operators.update(items => {
          items.forEach((operator) => {
            const skilledOperator = skilledOperators.find(
              (x) => x.id === operator.id
            );
            //the current operator has the skill
            //TODO: is the current operator available?
            if (skilledOperator) {
              operator.status = OperatorStatus.Available;
            } else {
              operator.status = OperatorStatus.NotQualified;
            }
          })
          return items;
        });

      });
  }

  private refreshResources(operatorId: string) {
    // resources that the operator has skill for
    this.appStore
      .getResourcesBasedOnOperator(operatorId)
      .then((resources) => (this.resources.set(resources)));
  }

  protected getShiftCalendarDays() {
    return this.data.calendarState.viewDatesStartingFrom(
      this.data.shift.startDate,
      this.shiftNumberOfdays()
    );
  }

  protected submit() {
    if (this.form.invalid || this.data.days.length === 0) {
      return;
    }
    if (!this.isOperatorSet()) {
      this.data.operator = this.operators().find(
        (x) => x.id === this.form.value.operatorId
      );
    }
    if (!this.isResourceSet()) {
      this.data.resource = this.resources().find(
        (x) => x.id === this.form.value.resourceId
      );
    }
    this.data.notes = this.form.value.notes ?? undefined;
    this.data.priority = this.form.value.priority ?? 0;

    this.dialogRef.close(this.data);
  }

  protected areOperatorStatusEquals(status1: OperatorStatus, status2: OperatorStatus) {
    return status1 === status2;
  }

  protected dayButtonClicked(calendarDate: CalendarDate) {
    const foundClickedDay = this.data.days.some(
      (x) => moment(x.date).diff(moment(calendarDate.date), 'days') === 0
    );
    if (foundClickedDay) {
      this.data.days = this.data.days.filter(
        (e) => moment(e.date).diff(moment(calendarDate.date), 'days') != 0
      );
    } else {
      this.data.days.push(calendarDate);
    }
  }

  protected isOperatorSet() {
    return this.data.operator != undefined;
  }

  protected isResourceSet() {
    return this.data.resource != undefined;
  }

}

