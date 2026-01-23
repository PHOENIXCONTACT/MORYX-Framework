/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import moment from 'moment';
import { DayOfTheWeek } from "./calendar-state";

export function dateToString(date: Date | undefined): string{
    if(!date) return "N/A";
    return date.toDateString();
}


//given a day of the week enum returns the name of that day ex: 'Monday'
export function getDayName(day: DayOfTheWeek): string {
    return DayOfTheWeek[day];
  }

  //given a day of the week enum returns the name of that day ex: 'Monday'
export function getShortDayName(day: DayOfTheWeek): string {
    return getDayName(day).slice(0,3);
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


  export function stringToDate(date?: string): Date {
    return moment(date).toDate();
 }
 
 // take a number and return a the value in 00 to 24 string 
export function formatDateDigits(number: number): string {
    return number <= 9 ? '0' + number : ''+number;
  }
  

  export function isSameDate(dateA: Date,dateB: Date){
    var flag = moment(dateA).isSame(moment(dateB),'date');
    return flag;
  }

  export function getDurationInDays(duration: string): number{
    return Number(duration?.split('.')[0] ?? 0)
  }
