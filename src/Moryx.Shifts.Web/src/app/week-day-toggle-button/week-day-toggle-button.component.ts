import { Component, EventEmitter, input, Input, output, Output } from '@angular/core';
import { CalendarDate, CalendarState } from '../models/calendar-state';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { formatDateDigits, getDayName, getShortDayName, isDayInInterval, localizedDayName, shortDayName } from '../utils';
import  moment from 'moment';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-week-day-toggle-button',
  templateUrl: './week-day-toggle-button.component.html',
  styleUrl: './week-day-toggle-button.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatButtonToggleModule,
    TranslateModule
  ]
})
export class WeekDayToggleButtonComponent {

  startDate = input.required<Date>();
  readOnly = input.required<boolean>();
  endDate = input.required<Date>();
  calendarState = input.required<CalendarState>();
  shiftNumberOfDay= input.required<number>();
  days = input.required<CalendarDate[]>();
  onButtonToggled = output<CalendarDate>();
  
  TranslationConstants = TranslationConstants;
  formatDateDigits = formatDateDigits;
  getDayName = getDayName;
  getShortDayName = getShortDayName;
  isDayInInterval = isDayInInterval;
  localizedDayName = localizedDayName;
  shortDayName = shortDayName;
  
  getCalendarDaysPerWeek() {
    let weeksAndDays: Array<CalendarDate[]> = [];
    const numberOfWeeks = Math.ceil(this.shiftNumberOfDay() / 7);
    let lastDate = this.startDate();
    for (let index = 1; index <= numberOfWeeks; index++) {
      const days = this.calendarState().viewDatesStartingFrom(lastDate, 7);
      weeksAndDays[index - 1] = days;
      lastDate = days[days.length - 1].date;
    }

    return weeksAndDays;
  }


  buttonToggled(event: MatButtonToggleChange) {
    this.onButtonToggled.emit(event.value);
  }

  isDaySelected(calendarDate: CalendarDate) {
    return this.days().some(
      (x) => moment(x.date).diff(moment(calendarDate.date), 'days') === 0
    );
  }


}
