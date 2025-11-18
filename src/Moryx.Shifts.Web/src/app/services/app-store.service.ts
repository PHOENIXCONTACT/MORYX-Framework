import { Injectable } from '@angular/core';
import { CalendarDate, CalendarState } from '../models/calendar-state';
import { BehaviorSubject, firstValueFrom, forkJoin } from 'rxjs';
import { OperatorModel } from '../models/operator-model';
import { OPERATORS, ORDERS, RESOURCES, SHIFTS } from '../models/dummy-data';
import { ShiftTypeModel } from '../models/shift-type-model';
import { ShiftInstanceModel } from '../models/shift-instance-model';
import { ShiftCardModel } from '../models/shift-card-model';
import { AssignmentService } from './assignment.service';
import { ShiftService } from './shift.service';
import moment from 'moment';
import { AssignmentCardModel } from '../models/assignment-card-model';
import AssignmentData from '../models/assignment-data';
import {
  hasDayInShiftInterval,
  isDayInInterval,
  secondsToHours,
  randomNumber,
} from '../utils';
import { ViewType } from '../models/types';
import { OrderModel } from '../models/order-model';
import {
  OperatorManagementService,
  OrderManagementService,
  ShiftManagementService,
} from '../api/services';
import {
  assignableOperatorToOperatorModel,
  assignmentToAssignmentCardModel,
  extendedAssignableOPeratorToOperatorModel,
  shiftInstanceToShiftCardModel,
  shiftTypeToShiftTypeModel,
} from '../models/model-converter';
import { AttendableResourceModel } from '../api/models/attendable-resource-model';
import { CopyShiftAndAssignmentData } from '../dialogs/copy-shift-and-assignment/copy-shift-and-assignment.component';

@Injectable({
  providedIn: 'root',
})
export class AppStoreService {
  // events

  extendedAssignableOPeratorToOperatorModel =
    extendedAssignableOPeratorToOperatorModel;
  private isOperatorFilterPanelOpened = new BehaviorSubject(false);
  private isResourceFilterPanelOpened = new BehaviorSubject(false);
  private isDraggingItem = new BehaviorSubject(false);
  private operatorsSelectedForFilter = new BehaviorSubject<OperatorModel[]>([]);
  private resourcesSelectedForFilter = new BehaviorSubject<AttendableResourceModel[]>([]);
  private droppableElementSearchString = new BehaviorSubject<
    string | undefined
  >(undefined);
  private searchOperatorInCalendarString = new BehaviorSubject<
    string | undefined
  >(undefined);
  private searchResourceInCalendarString = new BehaviorSubject<
    string | undefined
  >(undefined);
  private shifts = new BehaviorSubject<ShiftCardModel[]>([]);
  private currentView = new BehaviorSubject<ViewType>('Assignments');
  private orders = new BehaviorSubject<OrderModel[]>([]);
  private operators = new BehaviorSubject<OperatorModel[]>([]);
  private resources = new BehaviorSubject<AttendableResourceModel[]>([]);

  // events
  public isOperatorFilterPanelOpened$ =
    this.isOperatorFilterPanelOpened.asObservable();
  public isResourceFilterPanelOpened$ =
    this.isResourceFilterPanelOpened.asObservable();
  public operators$ = this.operators.asObservable();
  public resources$ = this.resources.asObservable();

  public shifts$ = this.shifts.asObservable();
  public isDraggingItem$ = this.isDraggingItem.asObservable();
  public operatorsSelectedForFilter$ =
    this.operatorsSelectedForFilter.asObservable();
  public resourcesSelectedForFilter$ =
    this.resourcesSelectedForFilter.asObservable();
  public droppableElementSearchString$ =
    this.droppableElementSearchString.asObservable();
  public searchOperatorInCalendarString$ =
    this.searchOperatorInCalendarString.asObservable();
  public searchResourceInCalendarString$ =
    this.searchResourceInCalendarString.asObservable();
  public shiftTypes$ = this.shiftService.shiftTypes.asObservable();
  public shiftInstances$ = this.shiftService.shiftInstances.asObservable();
  public assignments$ = this.assignmentService.assignments.asObservable();
  public currentView$ = this.currentView.asObservable();
  public orders$ = this.orders.asObservable();

  constructor(
    private assignmentService: AssignmentService,
    private shiftService: ShiftService,
    private shiftAssignmentService: ShiftManagementService,
    private operatorService: OperatorManagementService,
    private orderService: OrderManagementService
  ) {
    this.orderService.getOperations().subscribe((operations) => {
      const orderModels = operations.map(
        (x) =>
          <OrderModel>{
            operationNumber: x.number ?? '',
            orderNumber: x.order,
            totalHours: secondsToHours(x.targetCycleTime ?? 0),
            date: x.plannedStart ? moment(x.plannedStart) : new Date(),
          }
      );
      this.orders.next(orderModels);
    });

    this.shiftInstances$.subscribe((instances) => {
      //generate shift card models
      this.shifts.next(instances.map((x) => shiftInstanceToShiftCardModel(x)));
    });

    //fetch resources and operator elements from the API in parallel
    forkJoin([
      this.operatorService.getResources_1(),
      this.operatorService.getAll_5(),
    ]).subscribe((results) => {
      const resources = results[0];
      const operators = results[1];

      const resourcesModels = resources;
      this.resources.next(resourcesModels);

      const operatorModels = operators.map(assignableOperatorToOperatorModel);
      this.operators.next(operatorModels);

      this.shiftAssignmentService
        .getShiftAssignements()
        .subscribe((results) => {
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
    this.isOperatorFilterPanelOpened.next(
      !this.isOperatorFilterPanelOpened.value
    );
    this.isResourceFilterPanelOpened.next(false);
  }

  resourceFilterButtonClicked() {
    this.isResourceFilterPanelOpened.next(
      !this.isResourceFilterPanelOpened.value
    );
    this.isOperatorFilterPanelOpened.next(false);
  }

  dragItemFromShiftElementDrawer(dragging: boolean) {
    this.isDraggingItem.next(dragging);
  }

  async navigateToNextWeek(calendarState: CalendarState, format: string) {
    calendarState.next(format); // update the calendar to next week
  }

  async getCopyOfAssignmentAndShiftForPeriod(
    start: Date,
    end: Date,
    calendarState: CalendarState
  ) {
    const shiftsAndAssignment = <CopyShiftAndAssignmentData>{
      assignments: [],
      shiftInstances: [],
    };


    //find all shift instances for previous week
    return await this.getShiftInstancesForPeriod(start, end)
      .then(async (instances) => {
        const assignments = await firstValueFrom(this.assignmentService.assignments);
        const instancesForPreviousWeek = instances.filter(
          (x) =>
            isDayInInterval(x.startDate, start, end) ||
            isDayInInterval(x.endDate, start, end)
        );

        if (!instancesForPreviousWeek.length) return shiftsAndAssignment;

        for (let previousInstance of instancesForPreviousWeek) {
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
      });
  }

  async getShiftInstancesForPeriod(
    start: Date,
    end: Date): Promise<ShiftInstanceModel[]> {

    //find all shift instances
    const previousInstances = firstValueFrom(this.shiftService.shiftInstances);

    return await previousInstances.then(async (instances) => {
      const instancesFound = instances.filter(
        (x) =>
          isDayInInterval(x.startDate, start, end) ||
          isDayInInterval(x.endDate, start, end)
      );

      return instancesFound;
    });
  }

  async createNewAssignmentAndShift(data: CopyShiftAndAssignmentData) {
    //create new shift instance for the new week
    for (let newInstance of data.shiftInstances) {
      await this.shiftService.addInstance(newInstance)
        .then(async (instance) => {
          this.shiftService.addToInstanceList(instance);

          const assignments = data.assignments.filter(x => x.shift.id === newInstance.id); // based on temporary id
          for (let assignment of assignments) {
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
    if (
      !this.operatorsSelectedForFilter.value.some((x) => operator.id === x.id)
    )
      this.operatorsSelectedForFilter.next([
        ...this.operatorsSelectedForFilter.value,
        operator,
      ]);
    else
      this.operatorsSelectedForFilter.next(
        this.operatorsSelectedForFilter.value.filter((x) => x.id != operator.id)
      );
  }

  selectResource(resource: AttendableResourceModel) {
    if (
      !this.resourcesSelectedForFilter.value.some((x) => resource.id === x.id)
    )
      this.resourcesSelectedForFilter.next([
        ...this.resourcesSelectedForFilter.value,
        resource,
      ]);
    else
      this.resourcesSelectedForFilter.next(
        this.resourcesSelectedForFilter.value.filter((x) => x.id != resource.id)
      );
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
    this.currentView.next(view);
  }

  deleteAssignment(id: number) {
    this.assignmentService.delete(id);
  }

  updateShiftInstance(shiftInstance: ShiftInstanceModel) {
    this.shiftService.updateInstance(shiftInstance.id, shiftInstance);
  }

  getOperatorsBasedOnResource(
    resourceId: number
  ): Promise<Array<OperatorModel>> {
    var operatorsAsync = firstValueFrom(
      this.operatorService.getOperatorsByResource({ resourceId })
    );
    return operatorsAsync.then((operators) =>
      operators.map(extendedAssignableOPeratorToOperatorModel)
    );
  }

  getResourcesBasedOnOperator(
    operatorId: string
  ): Promise<Array<AttendableResourceModel>> {
    var resourcesAsync = firstValueFrom(
      this.operatorService.getResources({ operatorIdentifier: operatorId })
    );
    return resourcesAsync.then((resources) => resources);
  }
}
