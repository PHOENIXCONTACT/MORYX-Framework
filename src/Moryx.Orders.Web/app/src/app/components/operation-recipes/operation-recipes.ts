/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, computed, effect, OnInit, signal, untracked } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { environment } from "src/environments/environment";
import { OrderManagementService, ProductManagementService } from "../../api/services";
import { WorkplanService } from "../../api/services/workplan.service";
import { RecipeClassificationModel } from "../../api/models";
import { RecipeModel } from "../../api/models";
import { WorkplanModel } from "../../api/models";
import { OperationModel } from "../../api/models";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { CommonModule } from "@angular/common";
import { MatListModule } from "@angular/material/list";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatOptionModule } from "@angular/material/core";
import { MatInputModule } from "@angular/material/input";
import {MatSelectModule} from '@angular/material/select';

@Component({
  selector: "app-operation-recipes",
  templateUrl: "./operation-recipes.html",
  styleUrls: ["./operation-recipes.scss"],
  imports: [
    MatProgressBarModule,
    MatSidenavModule,
    MatToolbarModule,
    CommonModule,
    TranslateModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    FormsModule,
    MatOptionModule,
    NavigableEntryEditor,
    RouterLink,
    MatInputModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatSelectModule
  ],
  standalone: true,
})
export class OperationRecipes implements OnInit {
  recipes = signal<RecipeModel[]>([]);
  operation = signal<OperationModel>(<OperationModel>{});
  possibleWorkplans = signal<WorkplanModel[]>([]);
  isLoading = signal(false);
  isEditMode = signal(false);
  selectedWorkplan = signal<WorkplanModel | undefined>(undefined)
  isEditBarOpened = computed(
    () => !!this.selectedRecipe() && this.selectedRecipe()?.id! > 0
  );
  selectedRecipe = signal<RecipeModel | undefined>(undefined);
  hasWorkplans = computed(() => this.possibleWorkplans().length > 0);
  recipeClassifications: string[] = Object.keys(RecipeClassificationModel);
  operationRecipeToolbarImage: string =
    environment.assets + "assets/operation-recipe-editor.jpg";
  TranslationConstants = TranslationConstants;
  identifier: string = "";

  constructor(
    private activatedRoute: ActivatedRoute,
    private orderManagementService: OrderManagementService,
    private productManagementService: ProductManagementService,
    private workplanService: WorkplanService,
    public translate: TranslateService,
    private snackbarService: SnackbarService
  ) {
    effect(() => {
      const recipe = this.selectedRecipe();
      untracked(() => {
        if (recipe?.workplanModel)
          this.selectedWorkplan.set(this.getCurrentWorkplan(recipe));
      })
    })
  }

  async ngOnInit(): Promise<void> {
    this.isLoading.update((_) => true);
    this.activatedRoute.params.subscribe(async (params) => {
      this.identifier = params["identifier"];
      await this.workplanService
        .getAllWorkplans()
        .toAsync()
        .then((value) => this.possibleWorkplans.update((_) => value))
        .catch(
          async (e: HttpErrorResponse) =>
            await this.snackbarService.handleError(e)
        );
      await this.loadRecipes();
      this.isLoading.update((_) => false);
    });
  }
  getCurrentWorkplan(recipe: RecipeModel | undefined) {
    return this.possibleWorkplans()?.find(
      (w) => w.id === recipe?.workplanModel?.id
    );
  }
  private async loadRecipes(): Promise<void> {
    await this.orderManagementService
      .getOperation({ guid: this.identifier })
      .toAsync()
      .then((value) => this.operation.update((_) => value))
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    this.recipes.update((_) => []);
    for (const recipeId of this.operation().recipeIds!) {
      await this.productManagementService
        .getRecipe({ id: recipeId })
        .toAsync()
        .then(async (value) => {
          this.recipes.update((items) => {
            items.push(value);
            return items;
          });
          if (this.getCurrentWorkplan(value)) return;
          await this.fetchWorkplan(value);
        })
        .catch(
          async (e: HttpErrorResponse) =>
            await this.snackbarService.handleError(e)
        );
    }
    if (this.selectedRecipe())
      this.selectedRecipe.set(this.recipes().find((r) => r.id === this.selectedRecipe()?.id));
  }

  async fetchWorkplan(recipe: RecipeModel) {
    if (!recipe.workplanModel?.id) return;
    await this.workplanService
      .getWorkplan({ id: recipe.workplanModel?.id })
      .subscribe((value) => {
        this.possibleWorkplans.update((items) => {
          items.push(value);
          return items;
        });
      });
  }
  onSelect(recipe: RecipeModel) {
    this.selectedRecipe.update((_) => recipe);
  }

  onEdit() {
    this.isEditMode.update((_) => true);
  }

  async onSave() {
    this.isEditMode.update((_) => false);
    this.isLoading.update((_) => true);
    this.selectedRecipe.update((item) => {
      item!.workplanModel = this.selectedWorkplan();
      return item;
    });

    await this.productManagementService
      .updateRecipe({
        id: this.selectedRecipe()!.id!,
        body: this.selectedRecipe(),
      })
      .toAsync()
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    await this.loadRecipes();
    this.isLoading.update((_) => false);
  }

  async onCancel() {
    this.isEditMode.update((_) => false);
    this.isLoading.update((_) => true);
    await this.loadRecipes();
    this.isLoading.update((_) => false);
  }
}

