/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, computed, effect, inject, OnInit, signal, untracked, ChangeDetectionStrategy } from "@angular/core";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/translation-constants";
import { environment } from "../../../environments/environment";
import { OrderManagementService, ProductManagementService } from "@api/services";
import { WorkplanService } from "@api/services/workplan.service";
import { RecipeClassificationModel, RecipeModel, WorkplanModel, OperationModel } from "@api/models";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatListModule } from "@angular/material/list";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatOptionModule } from "@angular/material/core";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from '@angular/material/select';
import { MatTooltip } from '@angular/material/tooltip';

@Component({
  selector: "app-operation-recipes",
  templateUrl: "./operation-recipes.html",
  styleUrls: ["./operation-recipes.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatProgressBarModule,
    MatSidenavModule,
    MatToolbarModule,
    TranslatePipe,
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
    MatSelectModule,
    MatTooltip
  ]
})
export class OperationRecipes implements OnInit {
  protected recipes = signal<RecipeModel[]>([]);
  protected operation = signal<OperationModel>(<OperationModel>{});
  protected possibleWorkplans = signal<WorkplanModel[]>([]);
  protected isLoading = signal(false);
  protected isEditMode = signal(false);
  protected selectedWorkplan = signal<WorkplanModel | undefined>(undefined)
  protected isEditBarOpened = computed(
    () => !!this.selectedRecipe() && (this.selectedRecipe()?.id ?? 0) > 0
  );
  protected selectedRecipe = signal<RecipeModel | undefined>(undefined);
  protected hasWorkplans = computed(() => this.possibleWorkplans().length > 0);
  protected recipeClassifications: string[] = Object.keys(RecipeClassificationModel);
  protected operationRecipeToolbarImage: string =
    environment.assets + "assets/operation-recipe-editor.jpg";
  protected TranslationConstants = TranslationConstants;
  identifier: string = "";

  private activatedRoute = inject(ActivatedRoute);
  private orderManagementService = inject(OrderManagementService);
  private productManagementService = inject(ProductManagementService);
  private workplanService = inject(WorkplanService);
  private snackbarService = inject(SnackbarService);

  constructor() {
    effect(() => {
      const recipe = this.selectedRecipe();
      untracked(() => {
        if (recipe?.workplanModel) {
          this.selectedWorkplan.set(this.getCurrentWorkplan(recipe));
        }
      })
    })
  }

  async ngOnInit(): Promise<void> {
    this.isLoading.update((_) => true);
    this.activatedRoute.params.subscribe(async (params) => {
      this.identifier = params["identifier"];
      await this.workplanService
        .getAllWorkplans()
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
      .getOperation({guid: this.identifier})
      .then((value) => this.operation.update((_) => value))
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    this.recipes.update((_) => []);
    for (const recipeId of this.operation().recipeIds!) {
      await this.productManagementService
        .getRecipe({id: recipeId})
        .then(async (value) => {
          this.recipes.update((items) => {
            items.push(value);
            return items;
          });
          if (this.getCurrentWorkplan(value)) {
            return;
          }
          await this.fetchWorkplan(value);
        })
        .catch(
          async (e: HttpErrorResponse) =>
            await this.snackbarService.handleError(e)
        );
    }

    const selectedRecipe = this.selectedRecipe();

    if (selectedRecipe) {
      this.selectedRecipe.set(this.recipes().find((r) => r.id === selectedRecipe.id));
    }

    const updatedSelection = this.selectedRecipe();

    if (!updatedSelection && this.recipes().length > 0) {
     this.selectedRecipe.set(this.recipes()[0]);
    }
  }

  async fetchWorkplan(recipe: RecipeModel) {
    if (!recipe.workplanModel?.id) {
      return;
    }
    await this.workplanService
      .getWorkplan({id: recipe.workplanModel?.id})
      .then((value) => {
        this.possibleWorkplans.update((items) => {
          items.push(value);
          return items;
        });
      });
  }

  protected onSelect(recipe: RecipeModel) {
    this.selectedRecipe.update((_) => recipe);
  }

  protected onEdit() {
    this.isEditMode.update((_) => true);
  }

  protected async onSave() {
    this.isEditMode.update((_) => false);
    this.isLoading.update((_) => true);
    this.selectedRecipe.update((item) => {
      item!.workplanModel = this.selectedWorkplan();
      return item;
    });

    await this.productManagementService
      .updateRecipe({
        id: this.selectedRecipe()!.id!,
        body: this.selectedRecipe()
      })
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    await this.loadRecipes();
    this.isLoading.update((_) => false);
  }

  protected async onCancel() {
    this.isEditMode.update((_) => false);
    this.isLoading.update((_) => true);
    await this.loadRecipes();
    this.isLoading.update((_) => false);
  }
}

