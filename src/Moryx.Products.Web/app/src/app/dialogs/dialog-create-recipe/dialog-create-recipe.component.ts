/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from '@angular/core';
import { MAT_DIALOG_DEFAULT_OPTIONS, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { RecipeDefinitionModel, WorkplanModel } from '../../api/models';
import { CacheProductsService } from '../../services/cache-products.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatOptionModule } from '@angular/material/core';
import { MatListModule } from '@angular/material/list';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';

@Component({
    selector: 'app-dialog-create-recipe',
    templateUrl: './dialog-create-recipe.component.html',
    styleUrls: ['./dialog-create-recipe.component.scss'],
    standalone: true,
    imports: [
    MatFormFieldModule,
    FormsModule,
    MatOptionModule,
    TranslateModule,
    MatDialogModule,
    MatListModule,
    MatSelectModule,
    MatButtonModule,
    MatInputModule
],
})
export class DialogCreateRecipeComponent {
  result = signal<CreateRecipeDialogResult>({} as CreateRecipeDialogResult);
  possibleRecipes = signal<RecipeDefinitionModel[]>([]);
  possibleWorkplans = signal<WorkplanModel[]>([]);
  hasWorkplans = signal<boolean>(false);

  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<DialogCreateRecipeComponent>,
    cacheService: CacheProductsService,
    public translate: TranslateService
  ) {
    cacheService.recipeDefitions.subscribe((recipeDefintions) => {
      this.possibleRecipes.update(_=> recipeDefintions ?? []);
      if (this.possibleRecipes().length > 0
      ) {
        this.result.update(e => {
          e.selectedRecipe = this.possibleRecipes()[0]
          return e;
        });
        this.hasWorkplans.set(this.result()?.selectedRecipe?.hasWorkplans !== undefined);
      }
    });

    cacheService.workplans.subscribe((workplans) => {
      this.possibleWorkplans.set(workplans ?? []);
    });
  }

  onClose() {
    this.dialogRef.close();
  }

  onSelectedRecipeTypeChanged() {
    // Check if selected recipe type needs a workplan
    this.hasWorkplans.set(this.result()?.selectedRecipe?.hasWorkplans !== undefined);
  }
}

export interface CreateRecipeDialogResult {
  recipeName: string;
  selectedRecipe: RecipeDefinitionModel | undefined;
  workplanModel: WorkplanModel | undefined;
}

