import { Component, EventEmitter, Inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import AssignmentData from '../../models/assignment-data';
import { OPERATORS, RESOURCES } from 'src/app/models/dummy-data';
import { OperatorModel, OperatorStatus } from 'src/app/models/operator-model';
import {
  AbstractControl,
  FormArray,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DayOfTheWeek } from 'src/app/models/assignment-card-model';
import {
  formatDateDigits,
  getDayName,
  getShortDayName,
  isDayInInterval,
  localizedDayName,
  shortDayName,
} from 'src/app/utils';
import { MatButtonToggleChange } from '@angular/material/button-toggle';
import { CalendarDate } from 'src/app/models/calendar-state';
import  moment from 'moment';
import { AppStoreService } from 'src/app/services/app-store.service';
import { ResourceModel } from 'src/app/api/models/Moryx/Operators/Endpoints/resource-model';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { firstValueFrom } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ShiftInstanceModel } from 'src/app/models/shift-instance-model';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { WeekDayToggleButtonComponent } from 'src/app/week-day-toggle-button/week-day-toggle-button.component';

@Component({
  selector: 'app-week-assignment-dialog',
  templateUrl: './week-assignment-dialog.component.html',
  styleUrl: './week-assignment-dialog.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    WeekDayToggleButtonComponent
  ]
})
export class WeekAssignmentDialogComponent implements OnInit {
  operators = signal<OperatorModel[]>([]);
  resources = signal<ResourceModel[]>([]);
  shiftInstances = signal<ShiftInstanceModel[]>([]);
  shiftNumberOfdays = signal(0);

  available = OperatorStatus.Available;
  onVacation = OperatorStatus.OnVacation;
  notAllowed = OperatorStatus.NotAllowed;
  notQualified = OperatorStatus.NotQualified;
  TranslationConstants = TranslationConstants;
  form = new FormGroup({
    operatorId: new FormControl<string>('', [Validators.required]),
    resourceId: new FormControl<number>(0, [Validators.min(0)]),
    priority: new FormControl<number>(0, [Validators.min(0)]),
    notes: new FormControl(''),
  });
  formatDateDigits = formatDateDigits;
  getDayName = getDayName;
  getShortDayName = getShortDayName;
  isDayInInterval = isDayInInterval;
  localizedDayName = localizedDayName;
  shortDayName = shortDayName;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AssignmentData,
    public dialogRef: MatDialogRef<WeekAssignmentDialogComponent>,
    private appStore: AppStoreService
  ) {
    this.form.patchValue({
      operatorId: data.operator?.id ?? '',
      resourceId: data.resource?.id ?? -1,
      notes: data.notes ?? '',
      priority: data.priority ?? 0,
    });

    this.appStore.operators$.subscribe(
      (operators) => (this.operators.set(operators))
    );

    if (this.isResourceSet()) {
      this.appStore.resources$.subscribe((resources) => {
        this.resources.set( resources.filter((x) => x.id === data.resource?.id));
        this.form.controls.resourceId.disable();
        if (data.resource?.id) 
          this.refreshOperators(data.resource?.id);
      });
    } else {
      this.operators.update(items => items.filter((x) => x.id === data.operator?.id));
      this.form.controls.operatorId.disable();
      if (data.operator?.id)
        this.refreshResources(data.operator?.id);
    }

    const shiftInstancesAsync = firstValueFrom(this.appStore.shiftInstances$);
    shiftInstancesAsync.then((instances) => {
      this.shiftInstances.set(instances);
      const foundCurrentShift = instances.find((x) => x.id === data.shift.id);
      if (!foundCurrentShift) return;
      this.shiftNumberOfdays.set(foundCurrentShift.shiftType.duration);
    });
  }

  refreshOperators(resourceId: number) {
    //skilled operators
    this.appStore
      .getOperatorsBasedOnResource(resourceId)
      .then((skilledOperators) => {
        this.operators.update(items => {
          items.forEach((operator) => {
          let skilledOperator = skilledOperators.find(
            (x) => x.id === operator.id
          );
          //the current operator has the skill
          //TODO: is the current operator available?
          if (skilledOperator) operator.status = OperatorStatus.Available;
          else operator.status = OperatorStatus.NotQualified;
          })
          return items;
      });

      });
  }
  refreshResources(operatorId: string) {
    // resources that the operator has skill for
    this.appStore
      .getResourcesBasedOnOperator(operatorId)
      .then((resources) => (this.resources.set(resources)));
  }

  getShiftCalendarDays() {
    return this.data.calendarState.viewDatesStartingFrom(
      this.data.shift.startDate,
      this.shiftNumberOfdays()
    );
  }

  ngOnInit(): void {
    this.appStore.resources$.subscribe((resources) => {
      this.resources.set(resources);
    });
  }

  submit() {
    if (this.form.invalid || this.data.days.length === 0) return;
    if(!this.isOperatorSet())
      this.data.operator = this.operators().find(
        (x) => x.id === this.form.value.operatorId
      );
    if(!this.isResourceSet())
      this.data.resource = this.resources().find(
        (x) => x.id === this.form.value.resourceId
      );
    this.data.notes = this.form.value.notes ?? undefined;
    this.data.priority = this.form.value.priority ?? 0;

    this.dialogRef.close(this.data);
  }

  areOperatorStatusEquals(status1: OperatorStatus, status2: OperatorStatus) {
    return status1 === status2;
  }

  dayButtonClicked(calendarDate: CalendarDate) {
    var foundClickedDay = this.data.days.some(
      (x) => moment(x.date).diff(moment(calendarDate.date), 'days') === 0
    );
    if (foundClickedDay)
      this.data.days = this.data.days.filter(
        (e) => moment(e.date).diff(moment(calendarDate.date), 'days') != 0
      );
    else this.data.days.push(calendarDate);
  }

  isOperatorSet() {
    return this.data.operator != undefined;
  }

  isResourceSet() {
    return this.data.resource != undefined;
  }

}
