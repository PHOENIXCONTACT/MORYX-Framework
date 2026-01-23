/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  computed,
  effect,
  input,
  Input,
  signal,
  untracked,
} from "@angular/core";
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormControl,
} from "@angular/forms";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import {
  RecipeClassificationModel,
  RecipeModel,
  WorkplanModel,
} from "../../../../../api/models";
import { CacheProductsService } from "../../../../../services/cache-products.service";
import { CommonModule } from "@angular/common";
import { MatInput, MatInputModule } from "@angular/material/input";
import { MatOptionModule } from "@angular/material/core";
import { MatSelectModule } from "@angular/material/select";
@Component({
  selector: "app-product-recipes-details-header",
  templateUrl: "./product-recipes-details-header.component.html",
  styleUrls: ["./product-recipes-details-header.component.scss"],
  imports: [
    CommonModule,
    MatInputModule,
    TranslateModule,
    FormsModule,
    MatOptionModule,
    ReactiveFormsModule,
    MatInput,
    MatSelectModule
  ],
  standalone: true,
})
export class ProductRecipesDetailsHeaderComponent {
  edit = input.required<boolean>();
  recipe = input.required<RecipeModel>();
  hasWorkplans = computed(() => {
    if (this.recipe().workplanModel === undefined) return false;
    return true;
  });
  possibleWorkplans = signal<WorkplanModel[]>([]);
  recipeClassifications = signal(Object.keys(RecipeClassificationModel));

  recipeControl = new UntypedFormControl({
    value: RecipeClassificationModel.Unset,
  });
  TranslationConstants = TranslationConstants;

  constructor(
    private cacheService: CacheProductsService,
    public translate: TranslateService
  ) {
    effect(()=>{
      const edit = this.edit();
      untracked(() =>{
        if(edit)
          this.recipeControl.enable();
        else
          this.recipeControl.disable();
      })
    })
    this.cacheService.workplans.subscribe((workplans) => {
      this.possibleWorkplans.update((_) => workplans ?? []);
    });
  }

  byWorkplanId(workplan1: WorkplanModel , workplan2: WorkplanModel){
    return workplan1?.id === workplan2?.id;
  }
}

