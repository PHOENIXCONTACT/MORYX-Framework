/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RedirectCommand, ResolveFn, Router } from '@angular/router';
import { RecipeModel } from 'src/app/api/models';
import { EditProductsService } from 'src/app/services/edit-products.service';
import { toSignal } from '@angular/core/rxjs-interop';

/**
 * Sets the current recipe in the EditProductsService based on the recipeId route parameter. 
 * If the recipe is not found, it redirects to the product details page.
 */
export const RecipeResolver: ResolveFn<RecipeModel> = async (route: ActivatedRouteSnapshot) => {
  const editService = inject(EditProductsService);
  const router = inject(Router);
  const currentProduct = toSignal(editService.currentProduct$)();
  const recipeId = Number(route.paramMap.get('recipeId'));

  if (!currentProduct) {
    throw new Error('Invalid State: Tried to resolve product recipes without a current product');
  }

  const recipe = currentProduct.recipes?.find(r => r.id === recipeId);
  editService.setRecipe(recipe);
  if (recipe) {
    return recipe;
  }
  else {
    return new RedirectCommand(router.createUrlTree(['details', currentProduct.id]));
  }
};
