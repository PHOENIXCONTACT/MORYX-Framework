/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { CalendarDate, CalendarState } from '../models/calendar-state';
import { TranslationConstants } from '../translation-constants';
import { formatDateDigits, getDayName, getShortDayName, isDayInInterval, localizedDayName, shortDayName } from '../utils';
import  moment from 'moment';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-week-day-toggle-button',
  templateUrl: './week-day-toggle-button.html',
  styleUrl: './week-day-toggle-button.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatButtonToggleModule,
    TranslatePipe
  ]
})
export class WeekDayToggleButton {

  readonly startDate = input.required<Date>();
  readonly readOnly = input.required<boolean>();
  readonly endDate = input.required<Date>();
  readonly calendarState = input.required<CalendarState>();
  readonly shiftNumberOfDay= input.required<number>();
  readonly days = input.required<CalendarDate[]>();
  readonly buttonToggled = output<CalendarDate>();

  protected TranslationConstants = TranslationConstants;
  protected formatDateDigits = formatDateDigits;
  protected getDayName = getDayName;
  protected getShortDayName = getShortDayName;
  protected isDayInInterval = isDayInInterval;
  protected localizedDayName = localizedDayName;
  protected shortDayName = shortDayName;

  protected getCalendarDaysPerWeek() {
    const weeksAndDays: Array<CalendarDate[]> = [];
    const numberOfWeeks = Math.ceil(this.shiftNumberOfDay() / 7);
    let lastDate = this.startDate();
    for (let index = 1; index <= numberOfWeeks; index++) {
      const days = this.calendarState().viewDatesStartingFrom(lastDate, 7);
      weeksAndDays[index - 1] = days;
      lastDate = days[days.length - 1].date;
    }

    return weeksAndDays;
  }


  protected onButtonToggled(event: MatButtonToggleChange) {
    this.buttonToggled.emit(event.value);
  }

  protected isDaySelected(calendarDate: CalendarDate) {
    return this.days().some(
      (x) => moment(x.date).diff(moment(calendarDate.date), 'days') === 0
    );
  }


}

