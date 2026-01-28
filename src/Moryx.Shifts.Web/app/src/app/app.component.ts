/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, OnInit, signal } from '@angular/core';
import { CdkDragDrop, CdkDropList, DragDropModule } from '@angular/cdk/drag-drop';
import { OperatorModel, instanceOfOperator } from './models/operator-model';
import { CalendarDate, CalendarState } from './models/calendar-state';
import { AssignmentCardModel } from './models/assignment-card-model';
import { MatDialog } from '@angular/material/dialog';
import { ShiftCardModel } from './models/shift-card-model';
import AssignmentData from './models/assignment-data';
import { WeekAssignmentDialogComponent } from './dialogs/week-assignment-dialog/week-assignment-dialog.component';
import {
  hasDayInShiftInterval,
  formatDateDigits,
  isDayInInterval,
  totalOperatorForTheDay,
  totalHoursOfTheShift,
  shiftDayLengthInHours,
  localizedDayName,
  shortDayName,
  localizedFormatDate
} from './utils';
import { ShiftTypeDialogComponent } from './dialogs/shift-type-dialog/shift-type-dialog.component';
import { ShiftTypeModel } from './models/shift-type-model';
import { ShiftInstanceModel } from './models/shift-instance-model';
import { ShiftInstanceDialogComponent } from './dialogs/shift-instance-dialog/shift-instance-dialog.component';
import { AppStoreService } from './services/app-store.service';
import { FilterButtonType, ShiftElementTab, ViewType } from './models/types';
import {
  OrderModel,
  getOrderHoursForTheDay,
  getOrderOfTheDayBasedOnOperatorHours,
  getOrdersBasedOnOperatorHours,
  totalOrderHours
} from './models/order-model';
import { addCalendarDaysToAssignment } from './models/model-converter';
import { AttendableResourceModel } from './api/models/attendable-resource-model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { LanguageService, SnackbarService } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import {
  CopyShiftAndAssignmentComponent,
  CopyShiftAndAssignmentData
} from './dialogs/copy-shift-and-assignment/copy-shift-and-assignment.component';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { WorkHoursIconComponent } from './work-hours-icon/work-hours-icon.component';
import { AssignmentComponent } from './assignment/assignment.component';
import { OrderItemComponent } from './order-item/order-item.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    FormsModule,
    MatDividerModule,
    MatCheckboxModule,
    EmptyState,
    WorkHoursIconComponent,
    AssignmentComponent,
    OrderItemComponent,
    DragDropModule,
    TranslateModule
  ]
})
export class AppComponent implements OnInit {
  isOperatorFilterPanelOpened = signal(false);
  isResourceFilterPanelOpened = signal(false);
  operators = signal<OperatorModel[]>([]);
  resources = signal<AttendableResourceModel[]>([]);
  shifts = signal<ShiftCardModel[]>([]);
  operatorsSelectedForFilter = signal<OperatorModel[]>([]);
  resourcesSelectedForFilter = signal<AttendableResourceModel[]>([]);
  droppableElementSearchString = signal<string | undefined>(undefined);
  searchOperatorInCalendarString = signal<string | undefined>(undefined);
  searchResourceInCalendarString = signal<string | undefined>(undefined);
  shiftTypes = signal<ShiftTypeModel[]>([]);
  shiftInstances = signal<ShiftInstanceModel[]>([]);
  assignments = signal<AssignmentCardModel[]>([]);
  orders = signal<OrderModel[]>([]);
  currentView = signal<ViewType>('Assignments');
  drawerIsOpened = signal(false);
  selectedShiftElementTab = signal<ShiftElementTab>('Locations');
  isDraggingItem = signal(false);
  calendarState = signal<CalendarState | undefined>(undefined);
  filteredAssignments = computed(() => {
    const result = this.operatorsSelectedForFilter().length
      ? this.assignments().filter((a) =>
        this.operatorsSelectedForFilter().some((o) => o.id === a.operator.id)
      )
      : this.resourcesSelectedForFilter().length
        ? this.assignments().filter((a) =>
          this.resourcesSelectedForFilter().some((o) => o.id === a.resource.id)
        )
        : this.assignments();

    return result;
  })

  TranslationConstants = TranslationConstants;
  totalOperators = totalOperatorForTheDay;
  dayDropListId = 'day-list';
  weekDropListId = 'week-list';
  isDayInInterval = isDayInInterval;
  formatDateDigits = formatDateDigits;
  hasDayInShiftInterval = hasDayInShiftInterval;
  getOrderOfTheDayBasedOnOperatorHours = getOrderOfTheDayBasedOnOperatorHours;
  getOrderHoursForTheDay = getOrderHoursForTheDay;
  getOrdersBasedOnOperatorHours = getOrdersBasedOnOperatorHours;
  totalOrderHours = totalOrderHours;
  totalHoursOfTheShift = totalHoursOfTheShift;
  shiftDayLengthInHours = shiftDayLengthInHours;
  localizedDayName = localizedDayName;
  shortDayName = shortDayName;
  localizedFormatDate = localizedFormatDate;

  constructor(public dialog: MatDialog,
              public appStore: AppStoreService,
              public translate: TranslateService,
              public snackbarService: SnackbarService,
              private languageService: LanguageService) {

    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());

  }

  ngOnInit(): void {
    this.calendarState.set(new CalendarState(this.translate));

    //subscribe to store events (only source of truth)
    this.appStore.isOperatorFilterPanelOpened$.subscribe(
      value => this.isOperatorFilterPanelOpened.set(value)
    );
    this.appStore.isResourceFilterPanelOpened$.subscribe(
      value => this.isResourceFilterPanelOpened.set(value)
    );
    this.appStore.operators$.subscribe(
      values => this.operators.set(values)
    );
    this.appStore.resources$.subscribe(
      values => this.resources.set(values)
    );
    this.appStore.shifts$.subscribe(
      values => this.shifts.set(values)
    );
    this.appStore.shiftTypes$.subscribe(
      values => this.shiftTypes.set(values)
    );
    this.appStore.orders$.subscribe(orders => this.orders.set(orders));

    this.appStore.operatorsSelectedForFilter$
      .subscribe(selected => this.operatorsSelectedForFilter.set(selected));
    this.appStore.resourcesSelectedForFilter$
      .subscribe(selected => this.resourcesSelectedForFilter.set(selected));


    //get the assignments
    this.appStore.assignments$.subscribe(
      assignmentsReceived => this.assignments.set(assignmentsReceived)
    );

    this.appStore.shiftInstances$.subscribe(
      instances => {
        this.shiftInstances.set(instances);

        //get the assignments
        this.appStore.assignments$.subscribe(
          assignmentsReceived => {
            this.assignments.set(assignmentsReceived.map(x => {
              addCalendarDaysToAssignment(x, instances.find(e => e.id === x.shiftInstanceId));
              return x
            }))
          }
        );

      }
    );

    this.appStore.isDraggingItem$.subscribe(
      values => this.isDraggingItem.set(values)
    );

    this.appStore.currentView$.subscribe(value =>
      this.currentView.set(value)
    );
    this.appStore.orders$.subscribe(value =>
      this.orders.set(value)
    );
  }


  getFilterButtonStyle(button: FilterButtonType) {
    if (
      (this.isOperatorFilterPanelOpened() && button === 'Operator') ||
      (this.isResourceFilterPanelOpened() && button === 'Resource')
    )
      return 'flex flex-row items-center gap-x-2 px-2 py-1 border border-gray-200 border-solid rounded-sm bg-primary text-white';
    return 'flex flex-row items-center gap-x-2 px-2 py-1 border border-gray-200 border-solid hover:bg-gray-100 rounded-sm';
  }


  operatorFilterButtonClicked() {
    this.appStore.operatorFilterButtonClicked();
  }

  resourceFilterButtonClicked() {
    this.appStore.resourceFilterButtonClicked();
  }

  dragResourceOrOperator(dragging: boolean) {
    this.drawerIsOpened.set(false);
    this.appStore.dragItemFromShiftElementDrawer(dragging)

    if (!dragging) return;
  }

  dropElement(
    event: CdkDragDrop<AttendableResourceModel[] | OperatorModel[]>,
    shift?: ShiftCardModel,
    day?: CalendarDate
  ) {
    //the element was not drop on a different container
    if (event.previousContainer === event.container) return;
    if (event.container.id != this.dayDropListId && event.container.id != this.weekDropListId) return;
    this.handleAssignment(event, shift, day);
  }

  private handleAssignment(
    event: CdkDragDrop<AttendableResourceModel[] | OperatorModel[]>,
    shift?: ShiftCardModel,
    day?: CalendarDate
  ) {
    var operator: OperatorModel | undefined = undefined;
    var resource: AttendableResourceModel | undefined = undefined;
    //operator was dropped
    if (instanceOfOperator(event.item.data))
      operator = <OperatorModel>event.item.data;
    else resource = <AttendableResourceModel>event.item.data;

    this.translate
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {

      var dialogResult = this.dialog.open(WeekAssignmentDialogComponent, {
        data: <AssignmentData>{
          days: day != undefined ?
            [day] : [...this.calendarState()!.currentViewDates(),
              ...this.calendarState()!.viewDatesStartingFrom(this.calendarState()!.getNextWeek(translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]).startDate, 7)]
              .filter(x => isDayInInterval(x.date, shift?.startDate, shift?.endDate)),
          shift: shift,
          operator: operator,
          resource: resource,
          calendarState: this.calendarState()!
        },
      });

      dialogResult
        .afterClosed()
        .subscribe(async (weekAssigmentResult: AssignmentData) => {
          if (!weekAssigmentResult) return;

          const foundAssignment = this.filteredAssignments().find(
            (x) =>
              x.operator.id === weekAssigmentResult.operator?.id &&
              x.resource.id === weekAssigmentResult.resource?.id &&
              x.shiftInstanceId === shift?.id
          );
          this.appStore.handleAssignment(foundAssignment, weekAssigmentResult);
          this.translate
            .get([
              TranslationConstants.APP_COMPONENT.ASSIGNMENTS_SAVED,
            ]).subscribe(translations => {
            this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.ASSIGNMENTS_SAVED]);
          });
        });
    });
  }


  navigateToNextWeek() {
    this.translate
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {
      this.appStore.navigateToNextWeek(this.calendarState()!, translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]);
    });
  }

  navigateToPreviousWeek() {
    this.translate
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {
      this.appStore.navigateToPreviousWeek(this.calendarState()!, translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]);
    });
  }

  navigateToCurrentWeek() {
    this.appStore.navigateToCurrentWeek(this.calendarState()!);
  }

  selectOperator(operator: OperatorModel) {
    this.appStore.selectOperator(operator);
  }

  isOperatorSelected(operator: OperatorModel) {
    return this.operatorsSelectedForFilter().some((x) => x.id === operator.id);
  }

  isResourceSelected(resource: AttendableResourceModel) {
    return this.resourcesSelectedForFilter().some((x) => x.id === resource.id);
  }

  selectResource(resource: AttendableResourceModel) {
    this.appStore.selectResource(resource);
  }

  filteredResourceList(search: string | undefined | null) {
    if (search)
      return this.resources().filter((x) =>
        x.name?.toLowerCase().startsWith(search?.toLocaleLowerCase() ?? '')
      );
    return this.resources();
  }

  filterdOperatorList(search: string | undefined | null) {
    if (search)
      return this.operators().filter((x) => x.name.includes(search ?? ''));
    return this.operators();
  }

  addShiftType() {
    var dialogResult = this.dialog.open(ShiftTypeDialogComponent, {
      data: {},
    });

    dialogResult
      .afterClosed()
      .subscribe((shiftTypeResult: ShiftTypeModel) => {

        if (!shiftTypeResult) return;

        this.appStore.addShiftType(shiftTypeResult, this.calendarState()!);
        this.translate
          .get([
            TranslationConstants.APP_COMPONENT.SHIFT_CREATED,
          ]).subscribe(translations => {
          this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_CREATED]);
        })
      });
  }

  shiftInstancesForCurrentView(): ShiftCardModel[] {
    return this.shifts().filter(x => hasDayInShiftInterval(this.calendarState()!.currentViewDates(), x.startDate, x.endDate));
  }

  displayShiftInstance(shiftInstanceId: number) {
    const shiftInstance = this.shiftInstances().find(x => x.id === shiftInstanceId);
    if (!shiftInstance) return;

    var dialogResult = this.dialog.open(ShiftInstanceDialogComponent, {
      width: '500px',
      data: shiftInstance,
    });

    dialogResult
      .afterClosed()
      .subscribe((shiftInstanceResult: ShiftInstanceModel) => {
        if (!shiftInstanceResult) return;

        const shiftInstance = this.shiftInstances().find(x => x.id === shiftInstanceResult.id);
        if (!shiftInstance) return;
        shiftInstance.endDate = shiftInstanceResult.endDate;
        shiftInstance.startDate = shiftInstanceResult.startDate;
        this.appStore.updateShiftInstance(shiftInstance);
        this.translate
          .get([
            TranslationConstants.APP_COMPONENT.SHIFT_UPDATED,
          ]).subscribe(translations => {
          this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_UPDATED]);
        });
      });
  }

  getShiftInstanceById(id: number) {
    return this.shiftInstances().find(x => x.id === id);
  }

  changeView(view: ViewType) {
    this.appStore.changeView(view);
  }

  toggleDrawer() {
    this.drawerIsOpened.update(value => !value);
  }

  copyShiftAndAssignment() {
    this.appStore.getCopyOfAssignmentAndShiftForPeriod(this.calendarState()!.startDate, this.calendarState()!.endDate, this.calendarState()!)
      .then(assignmentsAndShift => {
        if (!assignmentsAndShift.shiftInstances.length) {
          this.translate
            .get([
              TranslationConstants.APP_COMPONENT.NO_SHIFT_TO_COPY,
            ]).subscribe(translations => {
            this.snackbarService.showError(translations[TranslationConstants.APP_COMPONENT.NO_SHIFT_TO_COPY]);
          });
          return;
        }
        var dialogResult = this.dialog.open(CopyShiftAndAssignmentComponent, {
          width: '700px',
          data: assignmentsAndShift,
        });

        dialogResult
          .afterClosed()
          .subscribe((shiftAndAssignmentsCopy: CopyShiftAndAssignmentData) => {
            if (!shiftAndAssignmentsCopy?.shiftInstances.length) return;

            this.appStore.createNewAssignmentAndShift(shiftAndAssignmentsCopy).then(() => {
              this.translate
                .get([
                  TranslationConstants.APP_COMPONENT.SHIFT_TO_COPIED,
                ]).subscribe(translations => {
                this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_TO_COPIED])
              })
            });
          });
      });
  }

}

