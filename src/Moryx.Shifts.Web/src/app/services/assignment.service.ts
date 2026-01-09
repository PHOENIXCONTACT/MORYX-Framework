import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { AssignmentCardModel } from '../models/assignment-card-model';
import AssignmentData from '../models/assignment-data';
import {
  OperatorManagementService,
  ShiftManagementService,
} from '../api/services';
import { ShiftAssignementModel } from '../api/models/shift-assignement-model';
import {
  calendarDatesToFlagEnumString
} from '../models/model-converter';
import { ShiftAssignementCreationContextModel } from '../api/models/shift-assignement-creation-context-model';

@Injectable({
  providedIn: 'root',
})
export class AssignmentService {


  public assignments = new BehaviorSubject<AssignmentCardModel[]>([]);

  constructor(
    private shiftAssignmentService: ShiftManagementService,
    private operatorService: OperatorManagementService
  ) {
  
  }

  public setAssignments(values : AssignmentCardModel[]){
    this.assignments.next(values);
  }

  public addNewAssignment(assignment: AssignmentData) {
    var newAssignment = <AssignmentCardModel>{
      days: assignment.days,
      resource: assignment.resource,
      notes: assignment.notes,
      priority: assignment.priority,
      operator: assignment.operator,
      shiftInstanceId: assignment.shift.id,
      id: 0
    };

    const data = <ShiftAssignementCreationContextModel>{
      note: newAssignment.notes,
      resourceId: newAssignment.resource.id,
      operatorIdentifier: newAssignment.operator.id,
      shiftId: newAssignment.shiftInstanceId,
      priority: newAssignment.priority ,
      assignedDays:  calendarDatesToFlagEnumString(assignment.days, assignment.shift.startDate,assignment.shift.endDate)
    }

    const assignmentAsync = firstValueFrom(this.shiftAssignmentService.createShiftAssignement({
      body: data
    }));

    return assignmentAsync.then(createdAssignment => {
          newAssignment.id = createdAssignment.id ?? 0;
          newAssignment.assignedDays = data.assignedDays ?? '';
          return newAssignment;
    })
  }

  public updateAssignment(assignmentId: number, update: AssignmentData) {
    const foundAssignment = this.assignments.value.find(
      (x: AssignmentCardModel) => x.id == assignmentId
    );

    if (foundAssignment) {
      var updated = <AssignmentCardModel>{
        days: update.days,
        id: assignmentId,
        resource: update.resource,
        notes: update.notes,
        priority: update.priority,
        operator: update.operator,
        shiftInstanceId: update.shift.id,
      };

      const data = <ShiftAssignementModel>{
        id: assignmentId,
        note: updated.notes,
        resourceId: updated.resource.id,
        operatorIdentifier: updated.operator.id,
        shiftId: updated.shiftInstanceId,
        priority: updated.priority,
        assignedDays:  calendarDatesToFlagEnumString(updated.days, update.shift.startDate,update.shift.endDate)
      }

      this.shiftAssignmentService.updateShiftAssignement({
        body: data
      }).subscribe( createdAssignment => {
        // this.assignments.next([
        //   ...this.assignments.value.filter((x) => x.id != assignmentId),
        //   updated,
        // ]);
        if(update.operator)
          foundAssignment.operator = update.operator;
        if(update.resource)
          foundAssignment.resource = update.resource;
        foundAssignment.days = update.days;
        foundAssignment.notes = update.notes;
        foundAssignment.priority = update.priority;
      })
     
    }
  }

  delete(id: number) {
     this.shiftAssignmentService.deleteShiftAssignement({
      id: id
     }).subscribe({
      next: succes =>{
         this.assignments.next(this.assignments.value.filter(x => x.id != id));
      },
      error: e => console.log(e)
     })
  }

  addAssignmentsToList(assignments: AssignmentCardModel[]){
    this.assignments.next([...this.assignments.value,...assignments]);
  }
}
