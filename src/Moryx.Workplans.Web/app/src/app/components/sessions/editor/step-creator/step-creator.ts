/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, input, OnDestroy, OnInit, output, signal, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { WorkplanStepRecipe } from '@api/models';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';

import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
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
export class StepCreator implements OnInit, OnDestroy {
  readonly availableSteps = input.required<WorkplanStepRecipe[]>();
  //TODO: remove this and change stepRecipe to type of model.required<..>() in future refactoring of the UI
  readonly created = output<WorkplanStepRecipe>();
  protected stepRecipe = signal<WorkplanStepRecipe | undefined>(undefined);
  recipeType = signal<String | undefined>(undefined);

  private activatedRoute = inject(ActivatedRoute);
  sub?: Subscription;
  protected readonly TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.sub = this.activatedRoute.queryParamMap.subscribe(m => {
      this.recipeType.update(_ => m.get('type') ?? undefined);
      this.stepRecipe.update(_ => structuredClone(this.availableSteps().find(s => s.type == this.recipeType())));
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  protected onCreate(): void {
    this.created.emit(this.stepRecipe()!);
  }
}

