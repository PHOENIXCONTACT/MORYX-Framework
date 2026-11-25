/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, input, linkedSignal, output, ChangeDetectionStrategy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { WorkplanStepRecipe } from '@api/models';
import { TranslationConstants } from '@app/translation-constants';

import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-step-creator',
  templateUrl: './step-creator.html',
  styleUrls: ['./step-creator.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    NavigableEntryEditor,
    TranslatePipe,
    MatSelectModule,
    MatButtonModule
  ]
})
export class StepCreator {
  readonly availableSteps = input.required<WorkplanStepRecipe[]>();
  //TODO: remove this and change stepRecipe to type of model.required<..>() in future refactoring of the UI
  readonly created = output<WorkplanStepRecipe>();

  private activatedRoute = inject(ActivatedRoute);
  private queryParamMap = toSignal(this.activatedRoute.queryParamMap);
  protected stepRecipe = linkedSignal(() => {
    const type = this.queryParamMap()?.get('type');
    return structuredClone(this.availableSteps().find(s => s.type == type));
  });
  protected readonly TranslationConstants = TranslationConstants;

  protected onParametersChange(parameters: Entry): void {
    this.stepRecipe.update(recipe => {
      if (!recipe?.constructor) {
        return recipe;
      }

      const constructor = {
        ...recipe.constructor,
        parameters: parameters
      };

      return {
        ...recipe,
        constructor: constructor
      };
    });
  }

  protected onCreate(): void {
    this.created.emit(this.stepRecipe()!);
  }
}

