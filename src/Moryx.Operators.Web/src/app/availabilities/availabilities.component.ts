/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component } from '@angular/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { MatDialog } from '@angular/material/dialog';
import { CalendarState } from '../models/calendar-state';
import { formatDateDigits, getDayName, getShortDayName, isDayInInterval, isSameDate } from '../models/utils';
import { AvailabilityDialogComponent } from '../dialogs/availability-dialog/availability-dialog.component';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-attandances',
    templateUrl: './availabilities.component.html',
    styleUrl: './availabilities.component.scss',
    standalone: true,
    imports: [
      CommonModule,
      MatIconModule,
      TranslateModule,
      MatButtonModule,
      RouterLink
    ]
})
export class AvailabilitiesComponent {
  TranslationConstants = TranslationConstants;
  calendarState!: CalendarState;
  getDayName = getDayName;
  isDayInInterval = isDayInInterval;
  formatDateDigits = formatDateDigits;
  getShortDayName = getShortDayName;
  isSameDate = isSameDate;
  constructor(private dialog: MatDialog){
    this.calendarState = new CalendarState();
  }

  onAddClick(){
    this.dialog.open(AvailabilityDialogComponent); 
  }

  navigateToCurrentWeek(){
    this.calendarState.reset();
  }

  navigateToPreviousWeek(){
    this.calendarState.previous();
  }

  navigateToNextWeek(){
    this.calendarState.next();
  }

  editAttandance(){

  }
}

