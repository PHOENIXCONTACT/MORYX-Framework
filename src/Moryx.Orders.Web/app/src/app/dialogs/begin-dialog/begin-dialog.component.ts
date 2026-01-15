/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, Inject, OnInit, signal, WritableSignal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { BeginModel } from '../../api/models';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperationViewModel } from 'src/app/models/operation-view-model';
import { FormControl, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RestrictionDescription } from '../../api/models';
import { OperatorsService } from 'src/app/services/operators.service';
import { AssignableOperator } from '../../api/models';
import { map, Observable, startWith } from 'rxjs';
import { BeginContext } from '../../api/models';
import { OperationStateClassification } from '../../api/models';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import { MatTooltip } from "@angular/material/tooltip";

@Component({
  selector: 'app-begin-dialog',
  templateUrl: './begin-dialog.component.html',
  styleUrls: ['./begin-dialog.component.scss'],
  standalone: true,
  imports: [
    MatDialogModule,
    CommonModule,
    TranslateModule,
    MatGridListModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    FormsModule,
    MatButtonModule,
    MatListModule,
    MatInputModule,
    MatButtonToggleModule,
    MatCardModule,
    MatAutocompleteModule,
    MatButtonToggleModule,
    MatTooltip
  ],
})
export class BeginDialogComponent implements OnInit {
  // Class properties for context values
  canBegin: boolean;
  canReduce: boolean;
  currentPartialAmount: number;
  successCount: number;
  scrapCount: number;
  runningCount: number;
  residualAmount: number;
  minimalTargetAmount: number;
  restrictions: RestrictionDescription[];
  operation: OperationViewModel;

  // Percentage values for progress bars
  successPercent = computed(() => (this.successCount * 100) / this.estimatedTotal());
  scrapPercent = computed(() => (this.scrapCount * 100) / this.estimatedTotal());
  partialPercent = computed(() => {
    const partial =
      (this.newTargetAmount() * 100) / this.estimatedTotal() - this.successPercent() - this.scrapPercent();
    return partial < 0 ? 0 : partial;
  });
  residualPercent = computed(() => 100 - this.successPercent() - this.scrapPercent() - this.partialPercent());
  private estimatedTotal = computed(() => {
    const residual = this.residualAmount > this.newPartialAmount() ? this.residualAmount - this.newPartialAmount() : 0;
    const active = this.scrapCount + this.successCount + this.runningCount;
    const current = this.newTargetAmount() > active ? this.newTargetAmount() : active;
    return current + residual;
  });

  overDeliveryReached = computed(() => {
    return this.operation.model.overDeliveryAmount
      ? this.newTargetAmount() > this.operation.model.overDeliveryAmount!
      : false;
  });
  underDeliveryReached = computed(() => {
    return this.operation.model.underDeliveryAmount
      ? this.newTargetAmount() < this.operation.model.underDeliveryAmount!
      : false;
  });

  hasMinimalValue = computed<boolean>(
    () => this.newPartialAmount() <= this.minimalTargetAmount - this.currentPartialAmount
  );

  newTargetAmount: WritableSignal<number>;
  private newPartialAmount = computed(() => this.newTargetAmount() - this.currentPartialAmount);

  targetAmountControl: FormControl;
  TranslationConstants = TranslationConstants;
  OperationStateClassification = OperationStateClassification;

  providesOperatorSelection: boolean = false;
  operatorFormControl = new FormControl('');
  operators: AssignableOperator[] = [];
  filteredOperators!: Observable<AssignableOperator[]>;

  constructor(
    public dialog: MatDialogRef<BeginDialogComponent, BeginModel | undefined>,
    @Inject(MAT_DIALOG_DATA) data: BeginDialogData,
    private operatorService: OperatorsService,
    public translate: TranslateService
  ) {
    this.canBegin = data.context.canBegin || false;
    this.canReduce = data.context.canReduce || false;
    this.currentPartialAmount = data.context.partialAmount || 0;
    this.successCount = data.context.successCount || 0;
    this.scrapCount = data.context.scrapCount || 0;
    this.runningCount = data.operation.model.runningCount || 0;
    this.residualAmount = data.context.residualAmount || 0;
    this.minimalTargetAmount = data.context.minimalTargetAmount || 0;
    this.restrictions = data.context.restrictions || [];
    this.operation = data.operation;
    this.newTargetAmount = signal(data.context.partialAmount || 0);

    this.targetAmountControl = new FormControl({
      value: this.currentPartialAmount,
      disabled: !this.canBegin && !this.canReduce,
    });
    this.targetAmountControl.valueChanges.subscribe(value => this.newTargetAmount.update(_ => value));
  }

  ngOnInit(): void {
    this.filteredOperators = this.operatorFormControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filter(value || ''))
    );

    this.operatorService.getOperators().then(o => {
      this.providesOperatorSelection = this.operatorService.available;
      this.operators = o;
    });

    if (!this.canBegin) this.operatorFormControl.disable();
  }

  limitTargetAmount() {
    if (this.targetAmountControl.value < this.minimalTargetAmount) {
      this.setMinTargetAmount();
    }
  }

  setMinTargetAmount() {
    this.targetAmountControl.setValue(this.minimalTargetAmount);
  }

  changeTargetAmount(change: number) {
    this.targetAmountControl.setValue(this.targetAmountControl.value + change);
  }

  private filter(value: string): AssignableOperator[] {
    const filterValue = value.toLowerCase();
    return this.operators.filter(
      o => o.pseudonym?.toLowerCase().includes(filterValue) || o.identifier?.toLocaleLowerCase().includes(filterValue)
    );
  }

  closeDialog() {
    const operatorIdentifier = this.operatorFormControl.value?.trim();
    if (!operatorIdentifier) {
      this.dialog.close({ amount: this.newPartialAmount() });
      return;
    }

    const selectedOperator = this.operators.find(o => o.identifier === operatorIdentifier);
    if (selectedOperator) {
      this.dialog.close({ amount: this.newPartialAmount(), userId: selectedOperator.identifier });
      return;
    }

    this.operatorService
      .addOperator(operatorIdentifier)
      .then(o => this.dialog.close({ amount: this.newPartialAmount(), userId: operatorIdentifier }))
      .catch(e =>
        this.operatorFormControl.setErrors({
          failed: e.error ?? 'An unknown error occured',
        } as ValidationErrors)
      );
  }
}

export interface BeginDialogData {
  context: BeginContext;
  operation: OperationViewModel;
}

