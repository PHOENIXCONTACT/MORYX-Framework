/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, input, signal, ChangeDetectionStrategy } from "@angular/core";
import { OperatorViewModel } from "../models/operator-view-model";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { WorkstationViewModel } from "../models/workstation-view-model";
import { ExtendedOperatorModel } from "@api/models/extended-operator-model";
import { AssignableOperator } from "@api/models/assignable-operator";
import { IOperatorAssignable } from "@api/models/i-operator-assignable";
import { AppStoreService } from "../services/app-store.service";
import { OperatorCard } from "../operator-card/operator-card";

import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: "app-operators",
  templateUrl: "./operators.html",
  styleUrl: "./operators.scss",
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    OperatorCard,
    EmptyState,
    TranslatePipe
  ]
})
export class Operators {

  readonly workstation = input.required<WorkstationViewModel>();
  readonly mainContainerStyle = input.required<string>();
  protected operators = signal<OperatorViewModel[]>([]);

  private appStoreService = inject(AppStoreService);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      this.appStoreService.operators();
      this.loadOperatorsByResource();
    });
  }

  private loadOperatorsByResource() {
    this.appStoreService
      .getOperatorsByResourceId(this.workstation()?.data.id ?? 0)
      .then(
        (skilledOperators: ExtendedOperatorModel[]) =>
          (this.operators.set(skilledOperators.map(
              (operator) =>
                new OperatorViewModel(<AssignableOperator>{
                  identifier: operator.identifier,
                  firstName: operator.firstName,
                  lastName: operator.lastName,
                  pseudonym: operator.pseudonym,
                  assignedResources: operator.assignedResources?.map(
                    (x) =>
                      <IOperatorAssignable>{
                        id: x.id,
                        name: x.name,
                      }
                  )
                })
            ))
          ));
  }

  protected async handleToggleAssignment(operator: OperatorViewModel) {
    if (!this.workstation()) {
      return;
    }

    //operator is already assigned to this resource so unassign the operator
    if (
      operator.data.assignedResources?.some(
        (e) => e.id === this.workstation()?.data.id
      )
    ) {
      this.appStoreService.unassignOperator(operator, this.workstation()!);
    } else {
      this.appStoreService.assignOperator(this.workstation()!, operator);
    }
  }
}
