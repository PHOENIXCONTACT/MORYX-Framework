/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom, Observable } from 'rxjs';
import { ShiftTypeModel } from '../models/shift-type-model';
import { ShiftCardModel } from '../models/shift-card-model';
import { ShiftInstanceModel } from '../models/shift-instance-model';
import { SHIFTS, SHIFT_INSTANCES, SHIFT_TYPES } from '../models/dummy-data';
import { ShiftManagementService } from '../api/services';
import { Time } from '@angular/common';
import { ShiftModel } from '../api/models/shift-model';
import { formatDateDigits } from '../utils';
import moment from 'moment';
import { ShiftCreationContextModel } from '../api/models/shift-creation-context-model';
import { ShiftTypeCreationContextModel } from '../api/models/shift-type-creation-context-model';
import {
  shiftToShitInstanceModel,
  shiftTypeToShiftTypeModel,
} from '../models/model-converter';

@Injectable({
  providedIn: 'root',
})
export class ShiftService {
  public shiftTypes = new BehaviorSubject<ShiftTypeModel[]>([]);
  public shiftInstances = new BehaviorSubject<ShiftInstanceModel[]>([]);

  constructor(private shiftManagement: ShiftManagementService) {
    //fetch shift types
    this.shiftManagement.getShiftTypes()
      .subscribe((shifts) => {
        const typeModels = shifts.map(shiftTypeToShiftTypeModel);
        this.shiftTypes.next(typeModels);

        // //fetch shift instances
        this.shiftManagement.getShifts()
          .subscribe((instanceModels) => {
            const instances = instanceModels.map((x) =>
              shiftToShitInstanceModel(typeModels, x)
            );
            this.shiftInstances.next(instances);
          });
      });
  }

  public addToInstanceList(instance: ShiftInstanceModel) {
    this.shiftInstances.next([...this.shiftInstances.value, instance]);
  }

  public addToTypeList(type: ShiftTypeModel) {
    this.shiftTypes.next([...this.shiftTypes.value, type]);
  }

  public addType(
    shift: ShiftTypeModel
  ) {
    //format the startTime & endTime to time format hh:mm:ss.ms
    const from = `${formatDateDigits(shift.startTime.hours)}:${formatDateDigits(
      shift.startTime.minutes
    )}:00.000`;
    const to = `${formatDateDigits(shift.endTime.hours)}:${formatDateDigits(
      shift.endTime.minutes
    )}:00.000`;
    const data = <ShiftTypeCreationContextModel>{
      name: shift.name,
      periode: shift.duration, //number of days
      startTime: from,
      endTime: to,
    };
    const typeAsync = firstValueFrom(this.shiftManagement.createShiftType({ body: data }));
    return typeAsync.then(typeResult => shiftTypeToShiftTypeModel(typeResult));
  }

  public addInstance(shift: ShiftInstanceModel) {
    const data = <ShiftCreationContextModel>{
      date: moment(shift.startDate).format('YYYY-MM-DD'),
      typeId: shift.shiftType.id,
    };

    const shiftInstanceAsync = firstValueFrom(this.shiftManagement.createShift({
      body: data,
    }));

    return shiftInstanceAsync.then(instance => shiftToShitInstanceModel(this.shiftTypes.value, instance));
  }

  updateInstance(id: number, shiftInstance: ShiftInstanceModel) {
    const update = <ShiftModel>{
      date: moment(shiftInstance.startDate).format('YYYY-MM-DD'),
      typeId: shiftInstance.shiftType.id,
      id: shiftInstance.id,
    };

    this.shiftManagement
      .updateShift({
        body: update,
      })
      .subscribe((result) => {
        const found = this.shiftInstances.value.find((x) => x.id === id);
        if (!found) return;
        found.startDate = shiftInstance.startDate;
        found.endDate = shiftInstance.endDate;
      });
  }
}

