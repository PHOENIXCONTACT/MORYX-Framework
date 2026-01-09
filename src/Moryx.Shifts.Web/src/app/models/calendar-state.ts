import  moment from 'moment';
import { DayOfTheWeek } from './assignment-card-model';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';

export interface CalendarModel {
  calendarWeek: number;
  startDate: Date;
  intervalDays: number;
  datesDisplayed: string;
}
export interface CalendarDate {
  day: DayOfTheWeek;
  date: Date;
}

export class CalendarState {
  private state: CalendarModel = <CalendarModel>{calendarWeek:0, startDate: new Date(), intervalDays:0, datesDisplayed: ''};
  private calendarDates: CalendarDate[] = [];
  private lang = 'en';
  
  constructor(private translate: TranslateService) {

      this.reset();

      //localize the calendar
      this.translate.onLangChange.subscribe(event => {
        this.lang = event.lang;
         this.reset();
      });
  }

  private initLang(){
    moment.locale(this.lang);
  }


  public reset() {
    this.translate
    .get([
      TranslationConstants.DATE_FORMAT.SHORT_DATE,
    ]).subscribe(translations => {
      const dateFormat: string = translations[TranslationConstants.DATE_FORMAT.SHORT_DATE];
      this.initLang();
      const now = moment();
      this.state = <CalendarModel>{
        calendarWeek: Number(now.format('W')),
        startDate: now.startOf('week').toDate(),
        intervalDays: 7,
        datesDisplayed: `${now.startOf('week').format(dateFormat)} - ${now
          .endOf('week').format(dateFormat)}`,
      };
  
      this.calendarDates = this.generateDates(this.state.intervalDays);
    });
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

  public currentViewDates(numberOfDays: number = 0) {
    if(numberOfDays>0) return this.generateDates(numberOfDays);
    return this.calendarDates;
  }

  public viewDatesStartingFrom(date: Date, numberOfDays: number = 0): CalendarDate[] {
    var lenght = 7;
    if(numberOfDays > 0) lenght = numberOfDays;
    return this.generateDates(lenght,date);
  }

  public getNextWeek(format: string): CalendarModel {
    this.initLang();
    const current = moment(this.state.startDate);
    
    const nextStartDate = current.endOf('week').add(1, 'days').startOf('week');
    return <CalendarModel>{
      calendarWeek: this.state.calendarWeek + 1,
      startDate: nextStartDate.toDate(),
      intervalDays: 7,
      datesDisplayed: `${nextStartDate
        .startOf('week')
        .format(format)} - ${nextStartDate
        .endOf('week')
        .format(format)}`,
    };
  }

  public next(format: string) {
    this.state = this.getNextWeek(format);
    this.calendarDates = this.generateDates(this.state.intervalDays);
  }

  public previous(format: string) {
    this.state = this.getPreviousWeek(format);
    this.calendarDates = this.generateDates(this.state.intervalDays);
  }

  public getPreviousWeek(format: string): CalendarModel {
    const current = moment(this.state.startDate);
    const startDate = current.startOf('week').add(-1, 'days').startOf('week');
    return <CalendarModel>{
      calendarWeek: this.state.calendarWeek - 1,
      startDate: startDate.toDate(),
      intervalDays: 7,
      datesDisplayed: `${startDate
        .startOf('week')
        .format(format)} - ${startDate
        .endOf('week')
        .format(format)}`,
    };
  }

  public generateDates(numberOfDays: number = 7, startDate?: Date): CalendarDate[] {
    this.initLang();
 
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
    this.initLang();
    const now = moment();
    return this.state?.calendarWeek === Number(now.format('W'));
  }
}
