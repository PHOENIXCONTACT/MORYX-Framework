/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AssignmentCardModel, DayOfTheWeek } from './models/assignment-card-model';
import { CalendarDate } from './models/calendar-state';
import moment from 'moment';
import { ShiftInstanceModel } from './models/shift-instance-model';
import { TranslationConstants } from './extensions/translation-constants.extensions';

//given a day of the week enum returns the name of that day ex: 'Monday'
export function getDayName(day: DayOfTheWeek): string {
  return DayOfTheWeek[day];
}

export function getShortDayName(day: DayOfTheWeek): string {
  return shortDayName(getDayName(day));
}

export function localizedDayName(dayOfTheWeek: DayOfTheWeek) {
  const day = DayOfTheWeek[dayOfTheWeek];
  for (const [key, value] of Object.entries(TranslationConstants.DAYS_OF_THE_WEEK)) {
    if (key === day) return value;
  }
  return TranslationConstants.DAYS_OF_THE_WEEK.Monday;
}

export function shortDayName(day: string) {
  return day.slice(0, 3);
}

//check if the given date is between the start date and end date
export function isDayInInterval(
  date: Date,
  startDate?: Date,
  endDate?: Date
): boolean {
  if (!startDate || !endDate) return false;

  const startMoment = moment(startDate);
  const endMoment = moment(endDate);

  return startMoment.isSameOrBefore(date) && endMoment.isSameOrAfter(date)
}

// checks if a date in the calendar view dates exist within the start and end dates of the shift.
export function hasDayInShiftInterval(
  calendarCurrentWeek: CalendarDate[],
  shiftStartDate?: Date,
  shiftEndDate?: Date
): boolean {
  if (!shiftStartDate || !shiftEndDate) return false;

  return calendarCurrentWeek.some(currentWeek => isDayInInterval(currentWeek.date, shiftStartDate, shiftEndDate));
}

// take a number and return a the value in 00 to 24 string
export function formatDateDigits(number: number): string {
  return number <= 9 ? '0' + number : '' + number;
}

export function localizedFormatDate(date: Date, format: string): string {
  const result = moment(date).format(format);
  return result;
}

// return the total work hours of the operators for the given calendar date
export function totalOperatorForTheDay(from: Date, to: Date,
                                       calendarDate: CalendarDate,
                                       assignements: AssignmentCardModel[],
                                       shiftInstance?: ShiftInstanceModel): number {
  if (!shiftInstance) return 0;
  var operators = assignements.filter(assignment => assignment.shiftInstanceId === shiftInstance.id && assignment.days
      .some(e => moment(e.date).diff(moment(calendarDate.date), 'days') === 0) &&
    isDayInInterval(calendarDate.date, shiftInstance.startDate, shiftInstance.endDate) && isDayInInterval(calendarDate.date, from, to)) ?? [];
  return operators.length ?? 0;
}

export function randomNumber(min: number, max: number) {
  return Math.floor((Math.random()) * (max - min + 1)) + min;
}

//how many hours of work exist for a given shift day
export function shiftDayLengthInHours(shiftInstance: ShiftInstanceModel | undefined, assignments: AssignmentCardModel[]): number {
  if (!shiftInstance) return 0;
  const startDate = moment(shiftInstance.startDate);
  // 00:00 is equal to 24:00 in moment.js library
  const from = startDate.format('MM/DD/YYYY') + ' ' + `${formatDateDigits(shiftInstance.shiftType.startTime.hours === 0 ? 24 : shiftInstance.shiftType.startTime.hours)}:${formatDateDigits(shiftInstance.shiftType.startTime.minutes)}`;
  const to = startDate.format('MM/DD/YYYY') + ' ' + `${formatDateDigits(shiftInstance.shiftType.endTime.hours === 0 ? 24 : shiftInstance.shiftType.endTime.hours)}:${formatDateDigits(shiftInstance.shiftType.endTime.minutes)}`;
  const startFrom = moment(from);
  const startTo = moment(to);
  return Math.abs(Number(startTo.diff(startFrom, 'hours')));
}

//returns total operators hours for a given shift
export function totalHoursOfTheShift(shiftInstance: ShiftInstanceModel | undefined, assignments: AssignmentCardModel[]) {

  if (!shiftInstance) return 0;
  const dailyShiftLengthHours = shiftDayLengthInHours(shiftInstance, assignments);
  let sumShiftHours = 0;
  for (let assignment of assignments.filter(x => x.shiftInstanceId === shiftInstance.id)) {
    sumShiftHours = sumShiftHours + assignment.days.length * dailyShiftLengthHours;
  }
  return sumShiftHours;
}

export function stringToDate(date?: string): Date {
  return moment(date).toDate();
}

export function secondsToHours(secs: number) {
  const hourTosecs = 3600;
  return secs / hourTosecs;
}

