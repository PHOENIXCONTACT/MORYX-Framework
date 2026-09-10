/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { OperatorModel, instanceOfOperator } from './models/operator-model';
import { CalendarDate, CalendarState } from './models/calendar-state';
import { MatDialog } from '@angular/material/dialog';
import { ShiftCardModel } from './models/shift-card-model';
import AssignmentData from './models/assignment-data';
import { WeekAssignmentDialog } from './dialogs/week-assignment-dialog/week-assignment-dialog';
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
import { ShiftTypeDialog } from './dialogs/shift-type-dialog/shift-type-dialog';
import { ShiftTypeModel } from './models/shift-type-model';
import { ShiftInstanceModel } from './models/shift-instance-model';
import { ShiftInstanceDialog } from './dialogs/shift-instance-dialog/shift-instance-dialog';
import { AppStoreService } from './services/app-store.service';
import { FilterButtonType, ShiftElementTab, ViewType } from './models/types';
import {
  getOrderHoursForTheDay,
  getOrderOfTheDayBasedOnOperatorHours,
  getOrdersBasedOnOperatorHours,
  totalOrderHours
} from './models/order-model';
import { addCalendarDaysToAssignment } from './models/model-converter';
import { AttendableResourceModel } from '@api/models/attendable-resource-model';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './translation-constants';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import {
  CopyShiftAndAssignment,
  CopyShiftAndAssignmentData
} from './dialogs/copy-shift-and-assignment/copy-shift-and-assignment';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTabsModule } from '@angular/material/tabs';
import { WorkHoursIcon } from './work-hours-icon/work-hours-icon';
import { Assignment } from './assignment/assignment';
import { OrderItem } from './order-item/order-item';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    FormsModule,
    MatDividerModule,
    MatCheckboxModule,
    MatTabsModule,
    EmptyState,
    WorkHoursIcon,
    Assignment,
    OrderItem,
    DragDropModule,
    TranslatePipe
  ]
})
export class App {
  private dialog = inject(MatDialog);
  private appStore = inject(AppStoreService);
  private translateService = inject(TranslateService);
  private snackbarService = inject(SnackbarService);

  protected isOperatorFilterPanelOpened = this.appStore.isOperatorFilterPanelOpened;
  protected isResourceFilterPanelOpened = this.appStore.isResourceFilterPanelOpened;
  protected operators = this.appStore.operators;
  protected resources = this.appStore.resources;
  protected shifts = this.appStore.shifts;
  protected operatorsSelectedForFilter = this.appStore.operatorsSelectedForFilter;
  protected resourcesSelectedForFilter = this.appStore.resourcesSelectedForFilter;
  protected droppableElementSearchString = signal<string | undefined>(undefined);
  protected searchOperatorInCalendarString = signal<string | undefined>(undefined);
  protected searchResourceInCalendarString = signal<string | undefined>(undefined);
  protected shiftTypes = this.appStore.shiftTypes;
  protected shiftInstances = this.appStore.shiftInstances;
  protected orders = this.appStore.orders;
  protected currentView = this.appStore.currentView;
  protected drawerIsOpened = signal(false);
  protected selectedShiftElementTab = signal<ShiftElementTab>('Locations');
  protected isDraggingItem = this.appStore.isDraggingItem;
  protected calendarState = signal<CalendarState | undefined>(undefined);
  protected assignments = computed(() => {
    const instances = this.appStore.shiftInstances();
    return this.appStore.assignments().map(x => {
      addCalendarDaysToAssignment(x, instances.find(e => e.id === x.shiftInstanceId));
      return x;
    });
  });
  protected filteredAssignments = computed(() => {
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

  protected TranslationConstants = TranslationConstants;
  protected totalOperators = totalOperatorForTheDay;
  protected dayDropListId = 'day-list';
  protected weekDropListId = 'week-list';
  protected isDayInInterval = isDayInInterval;
  protected formatDateDigits = formatDateDigits;
  protected hasDayInShiftInterval = hasDayInShiftInterval;
  protected getOrderOfTheDayBasedOnOperatorHours = getOrderOfTheDayBasedOnOperatorHours;
  protected getOrderHoursForTheDay = getOrderHoursForTheDay;
  protected getOrdersBasedOnOperatorHours = getOrdersBasedOnOperatorHours;
  protected totalOrderHours = totalOrderHours;
  protected totalHoursOfTheShift = totalHoursOfTheShift;
  protected shiftDayLengthInHours = shiftDayLengthInHours;
  protected localizedDayName = localizedDayName;
  protected shortDayName = shortDayName;
  protected localizedFormatDate = localizedFormatDate;

  constructor() {
    this.calendarState.set(new CalendarState(this.translateService));
  }


  protected getFilterButtonStyle(button: FilterButtonType) {
    if ((this.isOperatorFilterPanelOpened() && button === 'Operator') ||
      (this.isResourceFilterPanelOpened() && button === 'Resource')
    ) {
      return 'filter-button filter-button--active';
    }
    return 'filter-button';
  }


  protected operatorFilterButtonClicked() {
    this.appStore.operatorFilterButtonClicked();
  }

  protected resourceFilterButtonClicked() {
    this.appStore.resourceFilterButtonClicked();
  }

  protected dragResourceOrOperator(dragging: boolean) {
    this.appStore.dragItemFromShiftElementDrawer(dragging);

    if (!dragging) {
      this.drawerIsOpened.set(false);
    }
  }

  protected dropElement(
    event: CdkDragDrop<AttendableResourceModel[] | OperatorModel[]>,
    shift?: ShiftCardModel,
    day?: CalendarDate
  ) {
    //the element was not drop on a different container
    if (event.previousContainer === event.container) {
      return;
    }
    if (event.container.id != this.dayDropListId && event.container.id != this.weekDropListId) {
      return;
    }
    this.handleAssignment(event, shift, day);
  }

  private handleAssignment(
    event: CdkDragDrop<AttendableResourceModel[] | OperatorModel[]>,
    shift?: ShiftCardModel,
    day?: CalendarDate
  ) {
    let operator: OperatorModel | undefined = undefined;
    let resource: AttendableResourceModel | undefined = undefined;
    //operator was dropped
    if (instanceOfOperator(event.item.data)) {
      operator = <OperatorModel>event.item.data;
    } else {
      resource = <AttendableResourceModel>event.item.data;
    }

    this.translateService
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {

      const dialogResult = this.dialog.open(WeekAssignmentDialog, {
        data: <AssignmentData>{
          days: day != undefined ?
            [day] : [...this.calendarState()!.currentViewDates(),
              ...this.calendarState()!.viewDatesStartingFrom(this.calendarState()!.getNextWeek(translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]).startDate, 7)]
              .filter(x => isDayInInterval(x.date, shift?.startDate, shift?.endDate)),
          shift: shift,
          operator: operator,
          resource: resource,
          calendarState: this.calendarState()!
        }
      });

      dialogResult
        .afterClosed()
        .subscribe(async (weekAssigmentResult: AssignmentData) => {
          if (!weekAssigmentResult) {
            return;
          }

          const foundAssignment = this.filteredAssignments().find(
            (x) =>
              x.operator.id === weekAssigmentResult.operator?.id &&
              x.resource.id === weekAssigmentResult.resource?.id &&
              x.shiftInstanceId === shift?.id
          );
          this.appStore.handleAssignment(foundAssignment, weekAssigmentResult);
          this.translateService
            .get([
              TranslationConstants.APP_COMPONENT.ASSIGNMENTS_SAVED,
            ]).subscribe(translations => {
            this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.ASSIGNMENTS_SAVED]);
          });
        });
    });
  }


  protected navigateToNextWeek() {
    this.translateService
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {
      this.appStore.navigateToNextWeek(this.calendarState()!, translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]);
    });
  }

  protected navigateToPreviousWeek() {
    this.translateService
      .get([
        TranslationConstants.DATE_FORMAT.SHORT_DATE,
      ]).subscribe(translations => {
      this.appStore.navigateToPreviousWeek(this.calendarState()!, translations[TranslationConstants.DATE_FORMAT.SHORT_DATE]);
    });
  }

  protected navigateToCurrentWeek() {
    this.appStore.navigateToCurrentWeek(this.calendarState()!);
  }

  protected selectOperator(operator: OperatorModel) {
    this.appStore.selectOperator(operator);
  }

  protected isOperatorSelected(operator: OperatorModel) {
    return this.operatorsSelectedForFilter().some((x) => x.id === operator.id);
  }

  protected isResourceSelected(resource: AttendableResourceModel) {
    return this.resourcesSelectedForFilter().some((x) => x.id === resource.id);
  }

  protected selectResource(resource: AttendableResourceModel) {
    this.appStore.selectResource(resource);
  }

  protected filteredResourceList(search: string | undefined | null) {
    if (search) {
      return this.resources().filter((x) =>
        x.name?.toLowerCase().startsWith(search?.toLocaleLowerCase() ?? '')
      );
    }
    return this.resources();
  }

  protected filterdOperatorList(search: string | undefined | null) {
    if (search) {
      return this.operators().filter((x) => x.name.includes(search ?? ''));
    }
    return this.operators();
  }

  protected addShiftType() {
    const dialogResult = this.dialog.open(ShiftTypeDialog, {
      data: {}
    });

    dialogResult
      .afterClosed()
      .subscribe((shiftTypeResult: ShiftTypeModel) => {

        if (!shiftTypeResult) {
          return;
        }

        this.appStore.addShiftType(shiftTypeResult, this.calendarState()!);
        this.translateService
          .get([
            TranslationConstants.APP_COMPONENT.SHIFT_CREATED,
          ]).subscribe(translations => {
          this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_CREATED]);
        })
      });
  }

  protected shiftInstancesForCurrentView(): ShiftCardModel[] {
    return this.shifts().filter(x => hasDayInShiftInterval(this.calendarState()!.currentViewDates(), x.startDate, x.endDate));
  }

  protected displayShiftInstance(shiftInstanceId: number) {
    const shiftInstance = this.shiftInstances().find(x => x.id === shiftInstanceId);
    if (!shiftInstance) {
      return;
    }

    const dialogResult = this.dialog.open(ShiftInstanceDialog, {
      data: shiftInstance
    });

    dialogResult
      .afterClosed()
      .subscribe((shiftInstanceResult: ShiftInstanceModel) => {
        if (!shiftInstanceResult) {
          return;
        }

        const shiftInstance = this.shiftInstances().find(x => x.id === shiftInstanceResult.id);
        if (!shiftInstance) {
          return;
        }
        shiftInstance.endDate = shiftInstanceResult.endDate;
        shiftInstance.startDate = shiftInstanceResult.startDate;
        this.appStore.updateShiftInstance(shiftInstance);
        this.translateService
          .get([
            TranslationConstants.APP_COMPONENT.SHIFT_UPDATED,
          ]).subscribe(translations => {
          this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_UPDATED]);
        });
      });
  }

  protected getShiftInstanceById(id: number) {
    return this.shiftInstances().find(x => x.id === id);
  }

  protected changeView(view: ViewType) {
    this.appStore.changeView(view);
  }

  protected toggleDrawer() {
    this.drawerIsOpened.update(value => !value);
  }

  protected copyShiftAndAssignment() {
    const assignmentsAndShift = this.appStore.getCopyOfAssignmentAndShiftForPeriod(this.calendarState()!.startDate, this.calendarState()!.endDate, this.calendarState()!);
    if (!assignmentsAndShift.shiftInstances.length) {
      this.translateService
        .get([
          TranslationConstants.APP_COMPONENT.NO_SHIFT_TO_COPY,
        ]).subscribe(translations => {
        this.snackbarService.showError(translations[TranslationConstants.APP_COMPONENT.NO_SHIFT_TO_COPY]);
      });
      return;
    }
    const dialogResult = this.dialog.open(CopyShiftAndAssignment, {
      data: assignmentsAndShift
    });

    dialogResult
      .afterClosed()
      .subscribe((shiftAndAssignmentsCopy: CopyShiftAndAssignmentData) => {
        if (!shiftAndAssignmentsCopy?.shiftInstances.length) {
          return;
        }

        this.appStore.createNewAssignmentAndShift(shiftAndAssignmentsCopy).then(() => {
          this.translateService
            .get([
              TranslationConstants.APP_COMPONENT.SHIFT_TO_COPIED,
            ]).subscribe(translations => {
            this.snackbarService.showSuccess(translations[TranslationConstants.APP_COMPONENT.SHIFT_TO_COPIED])
          })
        });
      });
  }

}

