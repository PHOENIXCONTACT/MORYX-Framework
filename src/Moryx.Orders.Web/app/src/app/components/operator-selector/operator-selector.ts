/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, ChangeDetectionStrategy, DestroyRef, model, effect, input, output, signal } from "@angular/core";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { AssignableOperator } from "@api/models";
import { FormControl, ReactiveFormsModule, ValidationErrors } from "@angular/forms";
import { MatError, MatFormField, MatFormFieldModule, MatLabel } from "@angular/material/form-field";
import { MatOption } from '@angular/material/select';
import { map, Observable, startWith } from "rxjs";
import { OperatorsService } from "@app/services/operators.service";
import { AsyncPipe } from "@angular/common";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { MatInput } from "@angular/material/input";

@Component({
  selector: "app-operator-selector",
  templateUrl: "./operator-selector.html",
  styleUrls: ["./operator-selector.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatFormFieldModule,
    TranslatePipe,
    AsyncPipe,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatOption,
    MatFormField,
    MatLabel,
    MatError,
    MatInput
  ]
})
export class OperatorSelector implements OnInit {
  protected TranslationConstants = TranslationConstants;

  public isEnabled = input(true);
  public selectedOperatorId = model<string|null>(null);
  public creatingOperatorFailed = output<boolean>();

  protected providesOperatorSelection = signal<boolean>(false);
  protected operatorFormControl = new FormControl('');
  protected operators: AssignableOperator[] = [];
  protected filteredOperators!: Observable<AssignableOperator[]>;

  private operatorService = inject(OperatorsService);
  private destroyRef = inject(DestroyRef);

  constructor() {
    effect(() => {
      this.operatorFormControl.setValue(
        this.selectedOperatorId(),
        { emitEvent: false }
      );

      if (this.isEnabled()) {
        this.operatorFormControl.enable({ emitEvent: false });
      }
      else {
        this.operatorFormControl.disable({ emitEvent: false });
      }
    });
  }

  ngOnInit(): void {
    this.filteredOperators = this.operatorFormControl.valueChanges.pipe(
          startWith(''),
          map(value => this.filter(value || ''))
        );

    this.operatorService.getOperators().then(o => {
      this.providesOperatorSelection.set(this.operatorService.available);
      this.operators = o;
    });

    this.operatorFormControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(operatorIdentifier => this.handleValueChange(operatorIdentifier));
  }

  private async handleValueChange(operatorIdentifier : string | null) : Promise<void> {
    this.creatingOperatorFailed.emit(false);

    const trimedIdentifier = operatorIdentifier?.trim();

    if (trimedIdentifier) {
      const selectedOperator = this.operators.find(o => o.identifier === trimedIdentifier);

      if (!selectedOperator) {
        this.operatorService
          .addOperator(trimedIdentifier)
          .then(e => this.selectedOperatorId.set(trimedIdentifier))
          .catch(e => {
            this.operatorFormControl.setErrors({
              failed: e.error ?? 'An unknown error occured',
            } as ValidationErrors);
            this.selectedOperatorId.set(null);
             this.creatingOperatorFailed.emit(true);
          });
      }
      else {
        this.selectedOperatorId.set(trimedIdentifier);
      }
    }
    else {
      this.selectedOperatorId.set(null);
    }
  }

  private filter(value: string): AssignableOperator[] {
    const filterValue = value.toLowerCase();
    return this.operators.filter(
      o => o.pseudonym?.toLowerCase().includes(filterValue) || o.identifier?.toLocaleLowerCase().includes(filterValue)
    );
  }
}

