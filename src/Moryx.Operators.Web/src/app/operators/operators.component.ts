/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, Input, OnInit, signal } from "@angular/core";
import { OperatorViewModel } from "../models/operator-view-model";
import { Observable } from "rxjs";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { WorkstationViewModel } from "../models/workstation-view-model";
import { OperatorSkill } from "../models/operator-skill-model";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { ExtendedOperatorModel } from "../api/models/extended-operator-model";
import { AssignableOperator } from "../api/models/assignable-operator";
import { IOperatorAssignable } from "../api/models/i-operator-assignable";
import { AppStoreService } from "../services/app-store.service";
import { OperatorCardComponent } from "../operator-card/operator-card.component";
import { CommonModule } from "@angular/common";
import { EmptyStateComponent } from "@moryx/ngx-web-framework";
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: "app-operators",
    templateUrl: "./operators.component.html",
    styleUrl: "./operators.component.scss",
    standalone: true,
    imports:[
      OperatorCardComponent,
      CommonModule,
      EmptyStateComponent,
      TranslateModule
    ]
})
export class OperatorsComponent implements OnInit {
  
  workstation = input.required<WorkstationViewModel>();
  mainContainerStyle = input.required<string>();
  operators = signal<OperatorViewModel[]>([]);
  
  TranslationConstants = TranslationConstants;
  constructor(private appStoreService: AppStoreService) {}

  ngOnInit(): void {
    //load operators

    this.loadOperatorsByResource();
    this.appStoreService.operators$.subscribe((updatedOperators) =>
      this.loadOperatorsByResource()
    );
  }

  loadOperatorsByResource() {
    this.appStoreService
      .getOperatorsByResourceId(this.workstation()?.data.id ?? 0)
      .subscribe(
        (skilledOperators: ExtendedOperatorModel[]) =>
          (this.operators.update(_ => skilledOperators.map(
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
                ),
              })
          ))
      ));
  }

  async handleToggleAssignment(operator: OperatorViewModel) {
    if (!this.workstation()) return;

    //operator is already assigned to this resource so unassign the operator
    if (
      operator.data.assignedResources?.some(
        (e) => e.id === this.workstation()?.data.id
      )
    )
      this.appStoreService.unassignOperator(operator, this.workstation()!);
    else this.appStoreService.assignOperator(this.workstation()!, operator);
  }
}
