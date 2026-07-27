/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, ChangeDetectionStrategy } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { WorkplanNodeClassification, WorkplanStepRecipe } from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { SessionsService } from '@app/services/sessions.service';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-toolbox',
  templateUrl: './toolbox.html',
  styleUrls: ['./toolbox.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    MatExpansionModule,
    MatIconModule,
    MatCardModule,
    TranslatePipe
  ]
})
export class Toolbox {
  private workplanEditing = inject(WorkplanEditingService);
  private snackbarService = inject(SnackbarService);
  private sessionService = inject(SessionsService);

  protected stepRecipes: WorkplanStepRecipe[] = [];
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const session = this.sessionService.activeSession();
      if (session) {
        this.getAvailableSteps();
      }
    });
  }

  // ToDo: Add cache for available steps somewhere. They are fetched multiple times and also in the editor component
  private getAvailableSteps() {
    this.workplanEditing.availableSteps()
      .then(steps => (this.stepRecipes = steps))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected handleDragStart(stepRecipe: WorkplanStepRecipe, event: DragEvent) {
    event.dataTransfer?.setData('string', stepRecipe.type ? JSON.stringify(stepRecipe) : '');
  }

  protected getStepIcon(classification: WorkplanNodeClassification | undefined) {
    switch (classification) {
      case WorkplanNodeClassification.Input:
        return 'arrow_circle_down';
      case WorkplanNodeClassification.Output:
        return 'arrow_circle_up';
      case WorkplanNodeClassification.Execution:
        return 'commit';
      case WorkplanNodeClassification.Subworkplan:
        return 'account_tree';
      case WorkplanNodeClassification.ControlFlow:
        return 'alt_route';
      default:
        return 'question_mark';
    }
  }

  protected getControlFlow(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.ControlFlow);
  }

  protected getExecution(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.Execution);
  }

  protected getSubworkplans(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.Subworkplan);
  }

}

