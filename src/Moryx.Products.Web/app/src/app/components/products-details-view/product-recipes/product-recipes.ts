/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, inject, linkedSignal } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { MatDialog } from "@angular/material/dialog";
import { Router, RouterOutlet } from "@angular/router";
import { SnackbarService, } from "@moryx/ngx-web-framework/services";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { RecipeModel, WorkplanModel } from "../../../api/models";
import { ProductManagementService } from "../../../api/services";
import { DialogCreateRecipe } from "../../../dialogs/dialog-create-recipe/dialog-create-recipe";
import { EditProductsService } from "../../../services/edit-products.service";
import { MatListModule } from "@angular/material/list";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatExpansionModule } from "@angular/material/expansion";
import { lastValueFrom, map } from "rxjs";

@Component({
  selector: "app-product-recipes",
  templateUrl: "./product-recipes.html",
  styleUrls: ["./product-recipes.scss"],
  imports: [
    MatListModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    MatExpansionModule,
    RouterOutlet
  ]
})
export class ProductRecipes {
  private editProductsService = inject(EditProductsService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private productManagementService = inject(ProductManagementService);
  private snackbarService = inject(SnackbarService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  recipes = toSignal(this.editProductsService.currentProduct$.pipe(map(p => p?.recipes ?? []) ), { initialValue: [] });
  selectedRecipe = linkedSignal(this.editProductsService.currentRecipe);
  TranslationConstants = TranslationConstants;

  onAddRecipe() {
    const dialogRef = this.dialog.open(DialogCreateRecipe, {});

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      if (!result.selectedRecipe) return;

      this.createRecipe(result.recipeName, result.selectedRecipe.name, result.workplanModel);
    });
  }

  // ToDo: Move to edit service
  private async createRecipe(name: string, recipeType: string, workplanModel?: WorkplanModel) {
    let recipe: RecipeModel = {};
    try {
      recipe = await lastValueFrom(this.productManagementService.createRecipe({ recipeType: recipeType }));
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }

    recipe.name = name;
    recipe.workplanModel = workplanModel;
    this.editProductsService.currentRecipeNumber++;
    recipe.id = this.editProductsService.currentRecipeNumber;
    this.editProductsService.addRecipe(recipe);
    
    this.router.navigate(['details', this.editProductsService.currentProductId(), 'recipes', recipe.id]);
  }

  onSelect(recipe: RecipeModel) {
    if (this.selectedRecipe()?.id === recipe.id) {
      return;
    }

    this.router.navigate(['details', this.editProductsService.currentProductId(), 'recipes', recipe.id]);
  }

  onDeleteRecipe(event: Event, recipe: RecipeModel) {
    event.stopPropagation();
    this.editProductsService.removeRecipe(recipe);
    this.router.navigate(['details', this.editProductsService.currentProductId(), 'recipes']);
  }
}
