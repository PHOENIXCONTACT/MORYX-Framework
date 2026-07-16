/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject, Injectable, signal } from '@angular/core';
import { CalendarDate, CalendarState } from '../models/calendar-state';
import { OperatorModel } from '../models/operator-model';
import { ShiftTypeModel } from '../models/shift-type-model';
import { ShiftInstanceModel } from '../models/shift-instance-model';
import { AssignmentService } from './assignment.service';
import { ShiftService } from './shift.service';
import moment from 'moment';
import { AssignmentCardModel } from '../models/assignment-card-model';
import AssignmentData from '../models/assignment-data';
import { isDayInInterval, secondsToHours } from '../utils';
import { ViewType } from '../models/types';
import { OrderModel } from '../models/order-model';
import {
  OperatorManagementService,
  OrderManagementService,
  ShiftManagementService,
} from '@api/services';
import {
  assignableOperatorToOperatorModel,
  assignmentToAssignmentCardModel,
  extendedAssignableOPeratorToOperatorModel,
  shiftInstanceToShiftCardModel,

} from '../models/model-converter';
import { AttendableResourceModel } from '@api/models/attendable-resource-model';
import { CopyShiftAndAssignmentData } from '../dialogs/copy-shift-and-assignment/copy-shift-and-assignment';

@Injectable({
  providedIn: 'root',
})
export class AppStoreService {
  private assignmentService = inject(AssignmentService);
  private shiftService = inject(ShiftService);
  private shiftAssignmentService = inject(ShiftManagementService);
  private operatorManagementService = inject(OperatorManagementService);
  private orderManagementService = inject(OrderManagementService);

  private readonly _isOperatorFilterPanelOpened = signal(false);
  readonly isOperatorFilterPanelOpened = this._isOperatorFilterPanelOpened.asReadonly();
  private readonly _isResourceFilterPanelOpened = signal(false);
  readonly isResourceFilterPanelOpened = this._isResourceFilterPanelOpened.asReadonly();
  private readonly _isDraggingItem = signal(false);
  readonly isDraggingItem = this._isDraggingItem.asReadonly();
  private readonly _operatorsSelectedForFilter = signal<OperatorModel[]>([]);
  readonly operatorsSelectedForFilter = this._operatorsSelectedForFilter.asReadonly();
  private readonly _resourcesSelectedForFilter = signal<AttendableResourceModel[]>([]);
  readonly resourcesSelectedForFilter = this._resourcesSelectedForFilter.asReadonly();
  private readonly _currentView = signal<ViewType>('Assignments');
  readonly currentView = this._currentView.asReadonly();
  private readonly _orders = signal<OrderModel[]>([]);
  readonly orders = this._orders.asReadonly();
  private readonly _operators = signal<OperatorModel[]>([]);
  readonly operators = this._operators.asReadonly();
  private readonly _resources = signal<AttendableResourceModel[]>([]);
  readonly resources = this._resources.asReadonly();

  shifts = computed(() => this.shiftService.shiftInstances().map(shiftInstanceToShiftCardModel));
  shiftTypes = this.shiftService.shiftTypes;
  shiftInstances = this.shiftService.shiftInstances;
  assignments = this.assignmentService.assignments;

  constructor() {
    this.orderManagementService.getOperations().then((operations) => {
      const orderModels = operations.map(
        (x) =>
          <OrderModel>{
            operationNumber: x.number ?? '',
            orderNumber: x.order,
            totalHours: secondsToHours(x.targetCycleTime ?? 0),
            date: x.plannedStart ? moment(x.plannedStart) : new Date(),
          }
      );
      this._orders.set(orderModels);
    });

    //fetch resources and operator elements from the API in parallel
    Promise.all([
      this.operatorManagementService.getResources_1(),
      this.operatorManagementService.getAll(),
    ]).then((results) => {
      const resources = results[0];
      const operators = results[1];

      const resourcesModels = resources;
      this._resources.set(resourcesModels);

      const operatorModels = operators.map(assignableOperatorToOperatorModel);
      this._operators.set(operatorModels);

      this.shiftAssignmentService
        .getShiftAssignements()
        .then((results) => {
          this.assignmentService.setAssignments(
            results.map((x) =>
              assignmentToAssignmentCardModel(
                resourcesModels,
                operatorModels,
                x
              )
            )
          );
        });
    });
  }

  operatorFilterButtonClicked() {
    this._isOperatorFilterPanelOpened.set(!this._isOperatorFilterPanelOpened());
    this._isResourceFilterPanelOpened.set(false);
  }

  resourceFilterButtonClicked() {
    this._isResourceFilterPanelOpened.set(!this._isResourceFilterPanelOpened());
    this._isOperatorFilterPanelOpened.set(false);
  }

  dragItemFromShiftElementDrawer(dragging: boolean) {
    this._isDraggingItem.set(dragging);
  }

  async navigateToNextWeek(calendarState: CalendarState, format: string) {
    calendarState.next(format); // update the calendar to next week
  }

  getCopyOfAssignmentAndShiftForPeriod(
    start: Date,
    end: Date,
    calendarState: CalendarState
  ) {
    const shiftsAndAssignment = <CopyShiftAndAssignmentData>{
      assignments: [],
      shiftInstances: [],
    };

    //find all shift instances for previous week
    const instances = this.getShiftInstancesForPeriod(start, end);
    const assignments = this.assignmentService.assignments();

    if (!instances.length) {
      return shiftsAndAssignment;
    }

    for (const previousInstance of instances) {
      const newStartDate = moment(previousInstance.endDate).add(1, 'days');
      const newInstance = <ShiftInstanceModel>{
        id: previousInstance.id,// temporary id
        shiftType: previousInstance.shiftType,
        startDate: newStartDate.toDate(),
        endDate: newStartDate
          .add(previousInstance.shiftType.duration, 'days')
          .toDate(),
      };

      shiftsAndAssignment.shiftInstances.push(newInstance);
      const foundAssignments = assignments.filter((x) => x.shiftInstanceId === previousInstance.id);
      const foundAssignmentsData = foundAssignments.map((assignment) => {
        const assignmentData = <AssignmentData>{
          days: assignment.days.map(
            (x) =>
              <CalendarDate>{
                date: moment(x.date)
                  .add(previousInstance.shiftType.duration, 'days')
                  .toDate(),
                day: moment(x.date)
                  .add(previousInstance.shiftType.duration, 'days')
                  .day(),
              }
          ),
          resource: assignment.resource,
          operator: assignment.operator,
          shift: shiftInstanceToShiftCardModel(newInstance),
          notes: assignment.notes,
          priority: assignment.priority,
          calendarState: calendarState,
        };
        assignmentData.shift.id = previousInstance.id; //temporary id
        return assignmentData;
      });
      shiftsAndAssignment.assignments.push(...foundAssignmentsData);
    }

    return shiftsAndAssignment;
  }

  getShiftInstancesForPeriod(start: Date, end: Date): ShiftInstanceModel[] {
    return this.shiftService.shiftInstances().filter(
      (x) =>
        isDayInInterval(x.startDate, start, end) ||
        isDayInInterval(x.endDate, start, end)
    );
  }

  async createNewAssignmentAndShift(data: CopyShiftAndAssignmentData) {
    //create new shift instance for the new week
    for (const newInstance of data.shiftInstances) {
      await this.shiftService.addInstance(newInstance)
        .then(async (instance) => {
          this.shiftService.addToInstanceList(instance);

          const assignments = data.assignments.filter(x => x.shift.id === newInstance.id); // based on temporary id
          for (const assignment of assignments) {
            assignment.shift = shiftInstanceToShiftCardModel(instance);
            await this.assignmentService.addNewAssignment(assignment)
              .then(newAssignment => this.assignmentService.addAssignmentsToList([newAssignment]))
          }
        });
    }
  }

  navigateToPreviousWeek(calendarState: CalendarState, format: string) {
    calendarState.previous(format);
  }

  navigateToCurrentWeek(calendarState: CalendarState) {
    calendarState.reset();
  }

  selectOperator(operator: OperatorModel) {
    if (!this._operatorsSelectedForFilter().some((x) => operator.id === x.id)) {
      this._operatorsSelectedForFilter.update(current => [...current, operator]);
    } else {
      this._operatorsSelectedForFilter.update(current => current.filter((x) => x.id != operator.id));
    }
  }

  selectResource(resource: AttendableResourceModel) {
    if (!this._resourcesSelectedForFilter().some((x) => resource.id === x.id)) {
      this._resourcesSelectedForFilter.update(current => [...current, resource]);
    } else {
      this._resourcesSelectedForFilter.update(current => current.filter((x) => x.id != resource.id));
    }
  }

  addShiftType(shift: ShiftTypeModel, calendarState: CalendarState) {
    this.shiftService.addType(shift).then((type) => {
      this.shiftService.addToTypeList(type);
      shift.id = type.id;
      const shiftInstance = <ShiftInstanceModel>{
        shiftType: shift,
        startDate: calendarState.current.startDate,
        endDate: moment(calendarState.current.startDate)
          .add(shift.duration - 1, 'days')
          .toDate(),
      };
      this.shiftService.addInstance(shiftInstance).then((result) => {
        shiftInstance.id = result.id;
        this.shiftService.addToInstanceList(shiftInstance);
      });
    });
  }

  handleAssignment(
    assignment: AssignmentCardModel | undefined,
    data: AssignmentData
  ) {
    //assignment already exist
    if (assignment) {
      this.assignmentService.updateAssignment(assignment.id, data);
    } else {
      this.assignmentService.addNewAssignment(data).then((newAssignment) => {
        //add the newly created assignment to the list
        this.assignmentService.addAssignmentsToList([newAssignment]);
      });
    }
  }

  changeView(view: ViewType) {
    this._currentView.set(view);
  }

  deleteAssignment(id: number) {
    this.assignmentService.delete(id);
  }

  updateShiftInstance(shiftInstance: ShiftInstanceModel) {
    this.shiftService.updateInstance(shiftInstance.id, shiftInstance);
  }

  getOperatorsBasedOnResource(resourceId: number): Promise<Array<OperatorModel>> {
    return this.operatorManagementService.getOperatorsByResource({resourceId})
      .then((operators) => operators.map(extendedAssignableOPeratorToOperatorModel));
  }

  getResourcesBasedOnOperator(operatorId: string): Promise<Array<AttendableResourceModel>> {
    return this.operatorManagementService.getResources({operatorIdentifier: operatorId});
  }
}

