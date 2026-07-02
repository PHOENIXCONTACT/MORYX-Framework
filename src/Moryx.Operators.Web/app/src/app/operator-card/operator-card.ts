/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, input, output, ChangeDetectionStrategy } from '@angular/core';
import { OperatorViewModel } from '../models/operator-view-model';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-operator-card',
    templateUrl: './operator-card.html',
    styleUrl: './operator-card.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
      MatIconModule
    ]
})
export class OperatorCard {
  readonly workstationId = input.required<number>();
  readonly operator = input.required<OperatorViewModel>();
  readonly toggleAssignment = output<OperatorViewModel>();
  protected TranslationConstants = TranslationConstants;
  protected isAssigned = computed(() => this.operator().data.assignedResources?.some(x => x.id === this.workstationId()) );

  protected toggle(){
    this.toggleAssignment.emit(this.operator());
  }

}

