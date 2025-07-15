import { Time } from '@angular/common';
import { MoryxShiftsEndpointsShiftTypeModel } from '../api/models';
import { AssignableOperator } from '../api/models/Moryx/Operators/assignable-operator';
import { ShiftAssignementModel } from '../api/models/Moryx/Shifts/Endpoints/shift-assignement-model';
import { AssignmentCardModel } from './assignment-card-model';
import { OperatorModel, OperatorStatus } from './operator-model';
import { ShiftTypeModel } from './shift-type-model';
import { ShiftInstanceModel } from './shift-instance-model';
import { ShiftModel } from '../api/models/Moryx/Shifts/Endpoints/shift-model';
import { stringToDate } from '../utils';
import  moment from 'moment';
import { AssignedDays } from '../api/models/Moryx/Shifts/assigned-days';
import { CalendarDate } from './calendar-state';
import { PossibleAssignedDays } from './types';
import { ShiftCardModel } from './shift-card-model';
import { ResourceModel } from '../api/models/Moryx/Operators/Endpoints/resource-model';
import { ExtendedOperatorModel } from '../api/models/Moryx/Operators/Endpoints/extended-operator-model';



export function assignableOperatorToOperatorModel(
  operator: AssignableOperator
): OperatorModel {
  return <OperatorModel>{
    id: operator.identifier,
    name: operator.pseudonym,
    status: OperatorStatus.Available
  };
}

export function extendedAssignableOPeratorToOperatorModel(
  operator: ExtendedOperatorModel
): OperatorModel {
  return <OperatorModel>{
    id: operator.identifier,
    name: operator.pseudonym,
  };
}


export function assignmentToAssignmentCardModel(
  resources: ResourceModel[],
  operators: OperatorModel[],
  assignment: ShiftAssignementModel,
): AssignmentCardModel {
  const operator = operators.find(
    (x) => x.id === assignment.operatorIdentifier
  );
  const resource = resources.find((x) => x.id === assignment.resourceId);

  return <AssignmentCardModel>{
    id: assignment.id,
    notes: assignment.note,
    resource: resource,
    operator: operator,
    priority: assignment.priority,
    status: OperatorStatus.Available,
    shiftInstanceId: assignment.shiftId,
    assignedDays: assignment.assignedDays,
    days: [] // no days to set in this step. the day will be set in a different step
  };
}

export function addCalendarDaysToAssignment(
  model: AssignmentCardModel,
  shiftInstance: ShiftInstanceModel | undefined,
): AssignmentCardModel {

if(!shiftInstance) return model;

 model.days = assignedDaysToCalendarDates(shiftInstance.startDate,shiftInstance.endDate,model.assignedDays);
 return model;
}

export function assignedDaysToCalendarDates(
  startDate: Date,
  endDate: Date,
  assignedDays: string
): CalendarDate[] {
  let calendarDates: CalendarDate[] = [];
  
  if(!assignedDays) return calendarDates;

  const dayStringArray = assignedDays.split(',');
  const start = moment(startDate);
  const end = moment(endDate);

  for (
    let currentDate = start, day = 1;
    start.diff(end, 'days') <= 0;
    currentDate = currentDate.add(1, 'days'), day++
  ) {
    if (dayStringArray.includes('All')) {
      calendarDates.push(<CalendarDate>{
        date: currentDate.toDate(),
        day: currentDate.date(),
      });
      continue;
    }

    for (let dayString of dayStringArray) {
      const dayEnum = PossibleAssignedDays[day];
      const dayValue = dayString.replace(' ','');
      if(dayEnum === dayValue){
          calendarDates.push(<CalendarDate>{
            date: currentDate.toDate(),
            day: currentDate.date(),
          });
      }
    }
  }

  return calendarDates;
}

export function shiftTypeToShiftTypeModel(
  shiftType: MoryxShiftsEndpointsShiftTypeModel
): ShiftTypeModel {


      const data = <ShiftTypeModel>{
        id: shiftType.id,
        name: shiftType.name,
        duration: shiftType.periode,
        startTime: <Time>{
          hours: Number((<string>shiftType.startTime).split(':')[0]),
          minutes: Number((<string>shiftType.startTime).split(':')[1]),
        },
        endTime: <Time>{
          hours: Number((<string>shiftType.endtime).split(':')[0]),
          minutes: Number((<string>shiftType.endtime).split(':')[1]),
        },
      };
      return data;
}

export function shiftToShitInstanceModel(
  shiftTypes: ShiftTypeModel[],
  shift: ShiftModel
): ShiftInstanceModel {
  const type = shiftTypes.find((x) => x.id == shift.typeId);
  return <ShiftInstanceModel>{
    id: shift.id,
    shiftType: type,
    startDate: stringToDate(shift.date),
    endDate: moment(stringToDate(shift.date))
      .add((type?.duration ?? 1) - 1, 'days')
      .toDate(),
  };
}


export function shiftInstanceToShiftCardModel(instance : ShiftInstanceModel): ShiftCardModel{
  const shiftCard = <ShiftCardModel>{
    id: instance.id,
    shiftName: instance.shiftType.name,
    startTime: instance.shiftType.startTime,
    endTime: instance.shiftType.endTime,
    startDate: instance.startDate,
    endDate: instance.endDate,
  };
  return shiftCard;
}

export function calendarDatesToFlagEnumString(calendarDates: CalendarDate[],shiftStartDate: Date,shiftEndDate: Date): string{

  let stringResult = '';
const endDate = moment(shiftEndDate);
  for (let currentDate = moment(shiftStartDate),index = 1; currentDate.diff(endDate,'days') <= 0; currentDate = currentDate.add(1,'days'),index++) {
    
    const day = PossibleAssignedDays[index];
    const correspondingCalendarDate = calendarDates.find(x => moment(x.date).diff(currentDate) === 0);
    //the current date is not in the calendar for the shift
    if(!correspondingCalendarDate) continue;

    if(stringResult === '')
      stringResult+= `${day}`;
    else
      stringResult+= `,${day}`
  }
  return stringResult;
}