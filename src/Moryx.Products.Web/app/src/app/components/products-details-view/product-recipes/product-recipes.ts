/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, inject, OnInit, signal } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router, RouterOutlet } from "@angular/router";
import { SnackbarService, } from "@moryx/ngx-web-framework/services";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { RecipeModel } from "../../../api/models";
import { ProductManagementService } from "../../../api/services";
import { DialogCreateRecipeComponent } from "../../../dialogs/dialog-create-recipe/dialog-create-recipe";
import { EditProductsService } from "../../../services/edit-products.service";
import { CommonModule } from "@angular/common";
import { MatListModule } from "@angular/material/list";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatExpansionModule } from "@angular/material/expansion";

@Component({
  selector: "app-product-recipes",
  templateUrl: "./product-recipes.html",
  styleUrls: ["./product-recipes.scss"],
  imports: [
    CommonModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
    MatExpansionModule,
    RouterOutlet
  ]
})
export class ProductRecipes implements OnInit {
  editProductsService = inject(EditProductsService);
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private productManagementService = inject(ProductManagementService);
  private snackbarService = inject(SnackbarService);

  recipes = signal<Array<RecipeModel>>([]);
  selectedRecipe = signal<undefined | RecipeModel>(undefined);
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    const productId = this.activatedRoute.parent?.snapshot.paramMap.get("id");
    if (productId == null) return;

    this.editProductsService.currentProduct.subscribe((product) => {
      if (Number(productId) === product?.id) {
        if (product.recipes === null) this.recipes.update((_) => []);
        else {
          this.recipes.update((_) => product.recipes ?? []);
          let recipeId: String | null =
            product.recipes?.[0]?.id?.toString() ?? "";
          // Check if a recipe was already selected according to the activatedRoute
          const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
          if (!regexSpecificRecipe.test(this.router.url) && recipeId) {
            this.setSelectedRecipe(recipeId);
            let url = this.getBaseUrl();
            url += recipeId;
            this.router.navigate([url]);
            return;
          }
          recipeId = this.activatedRoute.children[0].snapshot.paramMap.get("recipeId");
          this.setSelectedRecipe(recipeId);
        }
      }
    });
  }

  setSelectedRecipe(recipeId: String | null) {
    this.selectedRecipe.update((_) =>
      this.recipes().find((recipe) => recipe.id === Number(recipeId))
    );
  }

  onAddRecipe() {
    const dialogRef = this.dialog.open(DialogCreateRecipeComponent, {});

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;
      if (!result.selectedRecipe) return;

      this.productManagementService
        .createRecipe({recipeType: result.selectedRecipe.name})
        .subscribe({
          next: (recipe) => {
            recipe.name = result.recipeName;
            recipe.workplanModel = result.workplanModel;
            this.editProductsService.currentRecipeNumber++;
            recipe.id = this.editProductsService.currentRecipeNumber;
            this.editProductsService.addRecipe(recipe);
            this.selectedRecipe.update((_) => recipe);
            let url = this.getBaseUrl();
            url += recipe.id;
            this.router.navigate([url]);
          },
          error: async (e: HttpErrorResponse) =>
            await this.snackbarService.handleError(e)
        });
    });
  }

  onSelect(recipe: RecipeModel) {
    if (this.selectedRecipe()?.id === recipe.id) return;

    this.selectedRecipe.update((_) => recipe);
    let url = this.getBaseUrl();
    if (recipe && recipe.id) {
      url += recipe.id;
    }
    this.router.navigate([url]);
  }

  onDeleteRecipe(event: Event, recipe: RecipeModel) {
    event.stopPropagation();
    this.editProductsService.removeRecipe(recipe);
  }

  // Get activatedRoute up to details/{productId}/recipes/
  private getBaseUrl(): string {
    const url = this.router.url;
    const index = url.lastIndexOf("recipes");
    let newUrl = url.substring(0, index);
    newUrl += "recipes/";
    return newUrl;
  }
}

