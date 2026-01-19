/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import moment from 'moment';

export interface CalendarModel {
    calendarWeek: number;
    startDate: Date;
    intervalDays: number;
    datesDisplayed: string;
  }

  
export enum DayOfTheWeek {
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 0
}
  export interface CalendarDate {
    day: DayOfTheWeek;
    date: Date;
  }
  
  export class CalendarState {
    private state!: CalendarModel;
    private calendarDates: CalendarDate[] = [];
    private now = moment();
    constructor() {
      this.reset();
    }
  
    public reset() {
      this.state = <CalendarModel>{
        calendarWeek: Number(this.now.format('W')),
        startDate: this.now.startOf('week').toDate(),
        intervalDays: 7,
        datesDisplayed: `${this.now.startOf('week').format('DD/MMM/YYYY')} - ${this.now
          .endOf('week')
          .format('DD/MMM/YYYY')}`,
      };
  
      this.calendarDates = this.generateDates(this.state.intervalDays);
    }
  
    public get current() {
      return this.state;
    }
  
    public get startDate(): Date {
      return this.state.startDate;
    }
  
    public get endDate(): Date {
      return moment(this.state.startDate).add(this.state.intervalDays,'days').toDate();
    }

    public get today(): Date{
        return moment().toDate();
    }
  
    public currentViewDates(numberOfDays: number = 0) {
      if(numberOfDays>0) return this.generateDates(numberOfDays);
      return this.calendarDates;
    }
  
    public viewDatesStartingFrom(date: Date, numberOfDays: number = 0): CalendarDate[] {
      var lenght = 7;
      if(numberOfDays > 0) lenght = numberOfDays;
      return this.generateDates(lenght,date);
    }
  
    public getNextWeek(): CalendarModel {
      const current = moment(this.state.startDate);
      const nextStartDate = current.endOf('week').add(1, 'days').startOf('week');
      return <CalendarModel>{
        calendarWeek: this.state.calendarWeek + 1,
        startDate: nextStartDate.toDate(),
        intervalDays: 7,
        datesDisplayed: `${nextStartDate
          .startOf('week')
          .format('DD/MMM/YYYY')} - ${nextStartDate
          .endOf('week')
          .format('DD/MMM/YYYY')}`,
      };
    }
  
    public next() {
      this.state = this.getNextWeek();
      this.calendarDates = this.generateDates(this.state.intervalDays);
    }
  
    public previous() {
      this.state = this.getPreviousWeek();
      this.calendarDates = this.generateDates(this.state.intervalDays);
    }
  
    public getPreviousWeek(): CalendarModel {
      const current = moment(this.state.startDate);
      const startDate = current.startOf('week').add(-1, 'days').startOf('week');
      return <CalendarModel>{
        calendarWeek: this.state.calendarWeek - 1,
        startDate: startDate.toDate(),
        intervalDays: 7,
        datesDisplayed: `${startDate
          .startOf('week')
          .format('DD/MMM/YYYY')} - ${startDate
          .endOf('week')
          .format('DD/MMM/YYYY')}`,
      };
    }
  
    public generateDates(numberOfDays: number = 7, startDate?: Date): CalendarDate[] {
      var dates : CalendarDate[] = [];
      var currentMoment: moment.Moment;
  
      if(!startDate) currentMoment =  moment(this.state.startDate);
      else currentMoment = moment(startDate);
  
      var end = moment(currentMoment).add(numberOfDays, 'days');
      for (
        let currentDate = currentMoment;
        currentDate.diff(end, 'days') < 0;
        currentDate = currentDate.add(1, 'days')
      ) {
        const calendarDate = <CalendarDate>{
          day: currentDate.day(),
          date: currentDate.toDate(),
        };
        dates.push(calendarDate);
      }
      return dates;
    }
  
    isCurrentWeek(){
      return this.state.calendarWeek === Number(this.now.format('W'));
    }
  }
  
