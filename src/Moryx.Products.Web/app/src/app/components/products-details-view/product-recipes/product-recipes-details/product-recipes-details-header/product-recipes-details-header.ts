/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  computed,
  effect,
  inject,
  linkedSignal,
  signal,
  untracked,
  ChangeDetectionStrategy
} from "@angular/core";
import { FormsModule, ReactiveFormsModule, UntypedFormControl } from "@angular/forms";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { RecipeClassificationModel, WorkplanModel } from "@api/models";
import { CacheProductsService } from "@app/services/cache-products.service";

import { MatInput, MatInputModule } from "@angular/material/input";
import { MatOptionModule } from "@angular/material/core";
import { MatSelectModule } from "@angular/material/select";
import { EditProductsService } from "@app/services/edit-products.service";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
import { RestrictKeywordValidatorDirective } from "./restrict-keyword-validator.directive";

@Component({
  selector: "app-product-recipes-details-header",
  templateUrl: "./product-recipes-details-header.html",
  styleUrls: ["./product-recipes-details-header.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatInputModule,
    TranslatePipe,
    FormsModule,
    MatOptionModule,
    ReactiveFormsModule,
    MatInput,
    MatSelectModule,
    EmptyState,
    RestrictKeywordValidatorDirective
  ]
})
export class ProductRecipesDetailsHeader {
  private cacheService = inject(CacheProductsService);
  private editProductsService = inject(EditProductsService);

  protected isEditMode = this.editProductsService.editing;
  protected currentRecipe = linkedSignal(this.editProductsService.currentRecipe);

  protected hasWorkplans = computed(() => {
    if (this.currentRecipe()?.workplanModel === undefined) {
      return false;
    }
    return true;
  });
  protected possibleWorkplans = this.cacheService.workplans;
  protected recipeClassifications = signal(Object.keys(RecipeClassificationModel));

  protected recipeControl = new UntypedFormControl({
    value: RecipeClassificationModel.Unset
  });
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const edit = this.isEditMode();
      untracked(() => {
        if (edit) {
          this.recipeControl.enable();
        } else {
          this.recipeControl.disable();
        }
      })
    });
  }

  protected byWorkplanId(workplan1: WorkplanModel, workplan2: WorkplanModel) {
    return workplan1?.id === workplan2?.id;
  }
}

