/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, EventEmitter, input, Input, OnDestroy, OnInit, output, Output, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { WorkplanStepRecipe } from '../../../../api/models';
import { TranslationConstants } from '../../../../extensions/translation-constants.extensions';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-step-creator',
  templateUrl: './step-creator.component.html',
  styleUrls: ['./step-creator.component.scss'],
  standalone: true,
  imports: [
    NavigableEntryEditor,
    TranslateModule,
    MatSelectModule,
    MatButtonModule
  ]
})
export class StepCreatorComponent implements OnInit, OnDestroy {
  availableSteps = input.required<WorkplanStepRecipe[]>();
  //TODO: remove this and change stepRecipe to type of model.required<..>() in future refactoring of the UI
  created = output<WorkplanStepRecipe>();
  stepRecipe = signal<WorkplanStepRecipe | undefined>(undefined);
  recipeType = signal<String | undefined>(undefined);

  sub?: Subscription;
  readonly TranslationConstants = TranslationConstants;

  constructor(private activatedRoute: ActivatedRoute, public translate: TranslateService) {
  }

  ngOnInit(): void {
    this.sub = this.activatedRoute.queryParamMap.subscribe(m => {
      this.recipeType.update(_ => m.get('type') ?? undefined);
      this.stepRecipe.update(_ => structuredClone(this.availableSteps().find(s => s.type == this.recipeType())));
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  onCreate(): void {
    this.created.emit(this.stepRecipe()!);
  }
}

