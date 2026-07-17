/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { ShiftTypeModel } from '../models/shift-type-model';
import { ShiftInstanceModel } from '../models/shift-instance-model';
import { ShiftManagementService } from '@api/services';
import { ShiftModel } from '@api/models/shift-model';
import { formatDateDigits } from '../utils';
import moment from 'moment';
import { ShiftCreationContextModel } from '@api/models/shift-creation-context-model';
import { ShiftTypeCreationContextModel } from '@api/models/shift-type-creation-context-model';
import { shiftToShitInstanceModel, shiftTypeToShiftTypeModel } from '../models/model-converter';

@Injectable({
  providedIn: 'root',
})
export class ShiftService {
  private shiftManagement = inject(ShiftManagementService);
  private readonly _shiftTypes = signal<ShiftTypeModel[]>([]);
  readonly shiftTypes = this._shiftTypes.asReadonly();
  private readonly _shiftInstances = signal<ShiftInstanceModel[]>([]);
  readonly shiftInstances = this._shiftInstances.asReadonly();

  constructor() {
    //fetch shift types
    this.shiftManagement.getShiftTypes()
      .then((shifts) => {
        const typeModels = shifts.map(shiftTypeToShiftTypeModel);
        this._shiftTypes.set(typeModels);

        // //fetch shift instances
        this.shiftManagement.getShifts()
          .then((instanceModels) => {
            const instances = instanceModels.map((x) =>
              shiftToShitInstanceModel(typeModels, x)
            );
            this._shiftInstances.set(instances);
          });
      });
  }

  public addToInstanceList(instance: ShiftInstanceModel) {
    this._shiftInstances.update(current => [...current, instance]);
  }

  public addToTypeList(type: ShiftTypeModel) {
    this._shiftTypes.update(current => [...current, type]);
  }

  public addType(shift: ShiftTypeModel) {
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
    return this.shiftManagement.createShiftType({ body: data })
      .then(typeResult => shiftTypeToShiftTypeModel(typeResult));
  }

  public addInstance(shift: ShiftInstanceModel) {
    const data = <ShiftCreationContextModel>{
      date: moment(shift.startDate).format('YYYY-MM-DD'),
      typeId: shift.shiftType.id,
    };

    return this.shiftManagement.createShift({ body: data })
      .then(instance => shiftToShitInstanceModel(this.shiftTypes(), instance));
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
      .then(() => {
        const found = this.shiftInstances().find((x) => x.id === id);
        if (!found) {
          return;
        }
        found.startDate = shiftInstance.startDate;
        found.endDate = shiftInstance.endDate;
      });
  }
}

