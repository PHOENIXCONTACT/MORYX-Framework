/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { RecipeDefinitionModel, WorkplanModel } from '../../api/models';
import { CacheProductsService } from '@app/services/cache-products.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-dialog-create-recipe',
  templateUrl: './dialog-create-recipe.html',
  styleUrls: ['./dialog-create-recipe.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatFormFieldModule,
    FormsModule,
    MatOptionModule,
    TranslatePipe,
    MatDialogModule,
    MatListModule,
    MatSelectModule,
    MatButtonModule,
    MatInputModule
  ]
})
export class DialogCreateRecipe {
  private dialogRef = inject(MatDialogRef<DialogCreateRecipe>);
  private cacheService = inject(CacheProductsService);

  protected result = signal<CreateRecipeDialogResult>({} as CreateRecipeDialogResult);
  protected possibleRecipes = computed(() => this.cacheService.recipeDefinitions() ?? []);
  protected possibleWorkplans = computed(() => this.cacheService.workplans() ?? []);
  protected hasWorkplans = signal<boolean>(false);

  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const recipes = this.possibleRecipes();
      untracked(() => {
        // Pre-select the first recipe when definitions become available
        if (recipes.length > 0) {
          this.result.update(e => ({ ...e, selectedRecipe: recipes[0] }));
          this.hasWorkplans.set(recipes[0]?.hasWorkplans !== undefined);
        }
      });
    });
  }

  protected onClose() {
    this.dialogRef.close();
  }

  protected onSelectedRecipeTypeChanged() {
    // Check if selected recipe type needs a workplan
    this.hasWorkplans.set(this.result()?.selectedRecipe?.hasWorkplans !== undefined);
  }
}

export interface CreateRecipeDialogResult {
  recipeName: string;
  selectedRecipe: RecipeDefinitionModel | undefined;
  workplanModel: WorkplanModel | undefined;
}

