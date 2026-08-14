/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, computed, inject, linkedSignal, ChangeDetectionStrategy } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Router, RouterOutlet } from "@angular/router";
import { SnackbarService, } from "@moryx/ngx-web-framework/services";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/translation-constants";
import { RecipeModel, WorkplanModel } from "../../../api/models";
import { ProductManagementService } from "../../../api/services";
import { DialogCreateRecipe } from "@app/dialogs/dialog-create-recipe/dialog-create-recipe";
import { EditProductsService } from "@app/services/edit-products.service";
import { MatListModule } from "@angular/material/list";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatExpansionModule } from "@angular/material/expansion";

@Component({
  selector: "app-product-recipes",
  templateUrl: "./product-recipes.html",
  styleUrls: ["./product-recipes.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatListModule,
    MatIconModule,
    MatButtonModule,
    TranslatePipe,
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

  protected isEditMode = this.editProductsService.editing;
  protected recipes = computed(() => this.editProductsService.currentProduct()?.recipes ?? []);
  protected selectedRecipe = linkedSignal(this.editProductsService.currentRecipe);
  protected TranslationConstants = TranslationConstants;

  protected onAddRecipe() {
    const dialogRef = this.dialog.open(DialogCreateRecipe, {});

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      if (!result.selectedRecipe) {
        return;
      }

      this.createRecipe(result.recipeName, result.selectedRecipe.name, result.workplanModel);
    });
  }

  // ToDo: Move to edit service
  private async createRecipe(name: string, recipeType: string, workplanModel?: WorkplanModel) {
    let recipe: RecipeModel = {};
    try {
      recipe = await this.productManagementService.createRecipe({ recipeType: recipeType });
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

  protected onSelect(recipe: RecipeModel) {
    if (this.selectedRecipe()?.id === recipe.id) {
      return;
    }

    this.router.navigate(['details', this.editProductsService.currentProductId(), 'recipes', recipe.id]);
  }

  protected onDeleteRecipe(event: Event, recipe: RecipeModel) {
    event.stopPropagation();
    this.editProductsService.removeRecipe(recipe);
    this.router.navigate(['details', this.editProductsService.currentProductId(), 'recipes']);
  }
}
