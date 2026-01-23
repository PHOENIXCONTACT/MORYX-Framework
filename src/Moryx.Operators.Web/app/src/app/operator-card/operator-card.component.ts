/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, EventEmitter, input, Input, output, Output } from '@angular/core';
import { OperatorViewModel } from '../models/operator-view-model';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-operator-card',
    templateUrl: './operator-card.component.html',
    styleUrl: './operator-card.component.scss',
    standalone: true,
    imports: [
      CommonModule,
      MatIconModule
    ]
})
export class OperatorCardComponent {
  workstationId = input.required<number>();
  operator = input.required<OperatorViewModel>();
  toggleAssignment = output<OperatorViewModel>();
  TranslationConstants = TranslationConstants;
  isAssigned = computed(() => this.operator().data.assignedResources?.some(x => x.id === this.workstationId()) );

  toggle(){
    this.toggleAssignment.emit(this.operator());
  }

}

