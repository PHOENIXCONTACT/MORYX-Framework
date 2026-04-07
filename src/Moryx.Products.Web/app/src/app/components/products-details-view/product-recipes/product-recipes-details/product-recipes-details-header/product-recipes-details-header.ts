/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, signal, untracked } from "@angular/core";
import { FormsModule, ReactiveFormsModule, UntypedFormControl } from "@angular/forms";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { RecipeClassificationModel, WorkplanModel } from "../../../../../api/models";
import { CacheProductsService } from "../../../../../services/cache-products.service";

import { MatInput, MatInputModule } from "@angular/material/input";
import { MatOptionModule } from "@angular/material/core";
import { MatSelectModule } from "@angular/material/select";
import { EditProductsService } from "src/app/services/edit-products.service";
import { toSignal } from "@angular/core/rxjs-interop";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";

@Component({
  selector: "app-product-recipes-details-header",
  templateUrl: "./product-recipes-details-header.html",
  styleUrls: ["./product-recipes-details-header.scss"],
  imports: [
    MatInputModule,
    TranslateModule,
    FormsModule,
    MatOptionModule,
    ReactiveFormsModule,
    MatInput,
    MatSelectModule,
    EmptyState
  ]
})
export class ProductRecipesDetailsHeader {
  private cacheService = inject(CacheProductsService);
  private editProductsService = inject(EditProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  currentRecipe = toSignal(this.editProductsService.currentRecipe$, { initialValue: undefined });
  
  hasWorkplans = computed(() => {
    if (this.currentRecipe()?.workplanModel === undefined) return false;
    return true;
  });
  possibleWorkplans = toSignal(this.cacheService.workplans, { initialValue: [] });
  recipeClassifications = signal(Object.keys(RecipeClassificationModel));

  recipeControl = new UntypedFormControl({
    value: RecipeClassificationModel.Unset
  });
  TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const edit = this.isEditMode();
      untracked(() => {
        if (edit)
          this.recipeControl.enable();
        else
          this.recipeControl.disable();
      })
    });
  }

  byWorkplanId(workplan1: WorkplanModel, workplan2: WorkplanModel) {
    return workplan1?.id === workplan2?.id;
  }
}

