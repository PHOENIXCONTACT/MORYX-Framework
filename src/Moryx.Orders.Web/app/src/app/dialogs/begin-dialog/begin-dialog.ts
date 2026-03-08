/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { BeginModel } from '../../api/models';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperationViewModel } from 'src/app/models/operation-view-model';
import { FormControl, FormsModule, ReactiveFormsModule, ValidationErrors } from '@angular/forms';
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
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MultiProgressBar } from "../../multi-progress-bar/multi-progress-bar";

@Component({
  selector: 'app-begin-dialog',
  templateUrl: './begin-dialog.html',
  styleUrls: ['./begin-dialog.scss'],
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
    MultiProgressBar
  ]
})
export class BeginDialog implements OnInit {
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

  // Count values for progress bar
  estimatedTotal = computed(() => {
    const residual = this.residualAmount > this.newPartialAmount() ? this.residualAmount - this.newPartialAmount() : 0;
    const active = this.scrapCount + this.successCount + this.runningCount;
    const current = this.newTargetAmount() > active ? this.newTargetAmount() : active;
    return current + residual;
  });
  partialCount = computed(() => {
    const partial = this.newTargetAmount() - this.successCount - this.scrapCount;
    return partial < 0 ? 0 : partial;
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

  private dialog = inject(MatDialogRef<BeginDialog, BeginModel | undefined>);
  private data = inject<BeginDialogData>(MAT_DIALOG_DATA);
  private operatorService = inject(OperatorsService);

  constructor() {
    this.canBegin = this.data.context.canBegin || false;
    this.canReduce = this.data.context.canReduce || false;
    this.currentPartialAmount = this.data.context.partialAmount || 0;
    this.successCount = this.data.context.successCount || 0;
    this.scrapCount = this.data.context.scrapCount || 0;
    this.runningCount = this.data.operation.model.runningCount || 0;
    this.residualAmount = this.data.context.residualAmount || 0;
    this.minimalTargetAmount = this.data.context.minimalTargetAmount || 0;
    this.restrictions = this.data.context.restrictions || [];
    this.operation = this.data.operation;
    this.newTargetAmount = signal(this.data.context.partialAmount || 0);

    this.targetAmountControl = new FormControl({
      value: this.currentPartialAmount,
      disabled: !this.canBegin && !this.canReduce
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
      this.dialog.close({amount: this.newPartialAmount()});
      return;
    }

    const selectedOperator = this.operators.find(o => o.identifier === operatorIdentifier);
    if (selectedOperator) {
      this.dialog.close({amount: this.newPartialAmount(), userId: selectedOperator.identifier});
      return;
    }

    this.operatorService
      .addOperator(operatorIdentifier)
      .then(o => this.dialog.close({amount: this.newPartialAmount(), userId: operatorIdentifier}))
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

