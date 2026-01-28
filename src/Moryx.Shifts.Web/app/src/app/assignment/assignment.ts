/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, EventEmitter, input, Input, model, Output, signal, untracked } from '@angular/core';
import {
  AssignmentCardModel,
  DayOfTheWeek,
} from '../models/assignment-card-model';
import { MatDialog } from '@angular/material/dialog';
import { ShiftCardModel } from '../models/shift-card-model';
import { OperatorModel, OperatorStatus } from '../models/operator-model';
import { WeekAssignmentDialog } from '../dialogs/week-assignment-dialog/week-assignment-dialog';
import AssignmentData from '../models/assignment-data';
import { CalendarDate, CalendarState } from '../models/calendar-state';
import { AppStoreService } from '../services/app-store.service';
import  moment from 'moment';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-assignment',
  templateUrl: './assignment.html',
  styleUrl: './assignment.scss',
  standalone: true,
  imports : [
    CommonModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    TranslateModule
  ]
})
export class Assignment {
  assignment = model.required<AssignmentCardModel>();
  calendarState = input.required<CalendarState>();
  calendarDate = input.required<CalendarDate>();
  shift = input.required<ShiftCardModel>();
  assignments  = signal<AssignmentCardModel[]>([]);

  TranslationConstants = TranslationConstants;
  notQualified = OperatorStatus.NotQualified;
  qualified = OperatorStatus.Available;

  constructor(public dialog: MatDialog,
    public appStore: AppStoreService ,   
    public translate: TranslateService,) 
  {
    this.appStore.assignments$.subscribe(
      values => this.assignments.set(values)
    );

    effect(() =>{
      const assignmentValue = this.assignment();
      untracked(() => {
        this.initialize(assignmentValue);
      })
    })
  }

  private initialize(value: AssignmentCardModel ){
    if(!value?.resource?.id) return;
    
    this.appStore
    .getOperatorsBasedOnResource(value.resource.id)
    .then((skilledOperators) => {
      let skilledOperator = skilledOperators.find(
        (x) => x.id === value.operator.id
      );

      if(!skilledOperator) {
        this.assignment.update(item => 
          {
            item.status = OperatorStatus.NotQualified;
            return item;
          });
        return;
      }

      this.assignment.update(item => 
        {
          item.status = OperatorStatus.Available;
          return item;
        });
      return;
    });
  }

  public assignmentIsForGivenDay(
    calendarDate: CalendarDate
  ): boolean {
    return this.assignment().days.some((x) => moment(x.date).diff(moment(calendarDate.date),'days') === 0);
  }

  showAssigmentDetails() {
    this.translate
    .get([
      TranslationConstants.DATE_FORMAT.SHORT_DATE,
    ]).subscribe(translations => {
      const assignmentData = <AssignmentData>{
        days: [...this.calendarState().currentViewDates(),...this.calendarState().viewDatesStartingFrom(this.calendarState().getNextWeek(translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]).startDate,7)]
        .filter(x => this.assignment().days.some(e => moment(e.date).isSame(moment(x.date),'days'))),
        shift: this.shift(),
        operator: this.assignment().operator,
        resource: this.assignment().resource,
        priority: this.assignment().priority,
        notes: this.assignment().notes,
        calendarState: this.calendarState()
      };
      var dialogResult = this.dialog.open(WeekAssignmentDialog, {
        data: assignmentData
      });
  
      dialogResult.afterClosed().subscribe((weekAssigmentResult: AssignmentData) => {
        if (!weekAssigmentResult) return;
        const foundAssignment = this.assignments().find(
          (x) =>
            x.operator.id === weekAssigmentResult.operator?.id &&
            x.resource.id === weekAssigmentResult.resource?.id &&
            x.shiftInstanceId === weekAssigmentResult.shift?.id 
        );
        this.appStore.handleAssignment(foundAssignment,weekAssigmentResult);
      });
    });
  }

  deleteAssignment(){
    this.appStore.deleteAssignment(this.assignment().id);
  }

}

