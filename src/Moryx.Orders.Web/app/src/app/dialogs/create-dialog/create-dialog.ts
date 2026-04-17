/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from "@angular/common";
import { Component, computed, effect, ElementRef, inject, linkedSignal, resource, signal, untracked, ViewChild } from "@angular/core";
import { FormControl, FormsModule, ReactiveFormsModule, UntypedFormControl } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatButtonModule } from "@angular/material/button";
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatSelectModule } from "@angular/material/select";
import { MatTooltipModule } from "@angular/material/tooltip";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { lastValueFrom } from "rxjs";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperationNumberValidations } from "src/app/validations/operationNumberValidations";
import { OperationCreationContextModel, ProductModel, ProductQuery, RecipeClassificationModel, RecipeFilter, RecipeModel, RevisionFilter } from '../../api/models';
import { OrderManagementService } from "../../api/services/order-management.service";
import { ProductManagementService } from "../../api/services/product-management.service";
import { HttpErrorResponse } from "@angular/common/http";

enum Action {
  AddCreate,
  AddOnly
}

@Component({
  selector: "app-create-dialog",
  templateUrl: "./create-dialog.html",
  styleUrls: ["./create-dialog.scss"],
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    FormsModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    MatButtonModule,
    MatSelectModule,
    CommonModule,
    MatIconModule,
    MatListModule,
    MatProgressBarModule,
    MatInputModule,
    MatMenuModule,
    MatTooltipModule,
    MatAutocompleteModule
  ],
  providers: []
})
export class CreateDialog {
  @ViewChild('productInput') productInput!: ElementRef<HTMLInputElement>;
  @ViewChild('recipeInput') recipeInput!: ElementRef<HTMLInputElement>;
    
  TranslationConstants = TranslationConstants;
  
  private orderManagementService = inject(OrderManagementService);
  private productManagementService = inject(ProductManagementService);
  private dialog = inject(MatDialogRef<CreateDialog>);
  private translateService = inject(TranslateService);
  private snackbarService = inject(SnackbarService);

  orderNumber = signal("");
  amount = signal(0);
  operations = signal<OperationCreationContextModel[]>([]);
  isLoading = signal(false);
  canAddOperation = computed(() => {
    const hasOrderNumber = this.orderNumber() !== "";
    const hasProduct = !!this.selectedProduct();
    const hasRecipe = !!this.selectedRecipe();
    const hasAmount = this.amount();

    const hasValidOperationNumber = this.operationNumberFormControl.value &&
      this.operationNumberFormControl.valid;

    return hasOrderNumber && hasValidOperationNumber && hasProduct && hasRecipe && hasAmount;
  });

  primaryAction = signal<Action>(Action.AddCreate);
  Action = Action;
  primaryActionLabel = computed(() => {
    return this.primaryAction() === Action.AddCreate
      ? this.translateService.instant(TranslationConstants.CREATE_DIALOG.CREATE)
      : this.translateService.instant(TranslationConstants.CREATE_DIALOG.ADD);
  });

  canAdd = computed(() => !this.isLoading() && this.canAddOperation());
  canCreate = computed(() => !(this.isLoading() || !(this.operations().length || this.canAddOperation())));
  canRun = computed<Record<Action, boolean>>(() => { return {
    [Action.AddCreate]: this.canCreate(),
    [Action.AddOnly]: this.canAdd()
  }});
  dropdownDisabled = computed(() => !this.canAdd() && !this.canCreate());

  private productsLoader = resource<ProductModel[], { query: ProductQuery }>({
    params: () => ({ query: 
      <ProductQuery>{
        recipeFilter: RecipeFilter.WithRecipe,
        revisionFilter: RevisionFilter.All,
      }}),
    loader: ({ params }) => lastValueFrom(this.productManagementService.getTypes({body: params.query}))
  });
  private possibleProducts = computed(() => {
    const error = this.productsLoader.error();
    if (error) {
      this.snackbarService.handleError(error as HttpErrorResponse)
      return [];
    }
    if (!this.productsLoader.hasValue()) {
      return [];
    }
    return this.productsLoader.value().sort((a, b) => this.byProductNameAndRevision(a, b));
  });
  filteredProducts = linkedSignal(this.possibleProducts)
  selectedProduct = signal<ProductModel | undefined>(undefined);  
  productFormControl = new FormControl<ProductModel | undefined>(undefined);

  private recipesLoader = resource<RecipeModel[], { product: ProductModel | undefined }>({
    params: () => ({ product: this.selectedProduct() }),
    loader: ({ params }) => {
      if (!params.product) {
        return Promise.resolve([]);
      }
      return this.loadRecipes(params.product.identifier!, params.product.revision!);
    }
  });
  private possibleRecipes = computed(() => {
    const error = this.recipesLoader.error();
    if (error) {
      this.snackbarService.handleError(error as HttpErrorResponse)
      return [];
    }
    if (!this.recipesLoader.hasValue()) {
      return [];
    }
    return this.recipesLoader.value();
  });
  filteredRecipes = linkedSignal(this.possibleRecipes)
  selectedRecipe = linkedSignal<RecipeModel[], RecipeModel | undefined>({
    source: this.possibleRecipes,
    computation: (recipes, previous) => recipes.find(r => r.id === previous?.value?.id)
  });
  recipeFormControl = new FormControl<RecipeModel | undefined>(undefined);

  operationNumberFormControl = new UntypedFormControl("", [
    OperationNumberValidations.isOperationNumberNotValid,
  ]);

  constructor() {
    effect(() => this.processLoading());
    effect(() => this.productFormControl.setValue(this.selectedProduct()));
    effect(() => this.recipeFormControl.setValue(this.selectedRecipe()));    
  }
  
  private processLoading(): void {
    const isLoading = this.productsLoader.isLoading() || this.recipesLoader.isLoading();
    const error = !!this.productsLoader.error() || !!this.recipesLoader.error() ;
    
    untracked(() => {
      this.isLoading.set(isLoading);
      
      if(isLoading)
        this.operationNumberFormControl.disable();
      else
        this.operationNumberFormControl.enable();

      if (error) {
        this.dialog.close();
      }
    });
  }

  detailsInputchanged(event: Event | KeyboardEvent) {
    if (this.operations().length > 0) {
      this.addValidationToOperationNumber();
    }
  }

  addOperation(): void {
    const operation = <OperationCreationContextModel>{};
    operation.operationNumber = this.operationNumberFormControl.value;
    operation.name = this.selectedProduct()?.name;
    operation.productIdentifier = this.selectedProduct()?.identifier;
    operation.productRevision = this.selectedProduct()?.revision;
    operation.recipePreselection = this.selectedRecipe()?.id;
    operation.totalAmount = this.amount();
    this.operations.update((items) => {
      items.push(operation);
      return items;
    });
    this.clearOperation();
  }

  deleteOperation(operation: OperationCreationContextModel): void {
    let index = this.operations().indexOf(operation);
    this.operations.update((items) => {
      items.splice(index, 1);
      return items;
    });
    if (this.operations().length === 0) this.addValidationToOperationNumber();
  }

  addValidationToOperationNumber() {
    if (
      this.operationNumberFormControl.hasValidator(
        OperationNumberValidations.isOperationNumberNotValid
      )
    )
      return;

    this.operationNumberFormControl.addValidators(
      OperationNumberValidations.isOperationNumberNotValid
    );
    this.operationNumberFormControl.updateValueAndValidity({
      onlySelf: true
    });
  }

  fullClear(): void {
    this.orderNumber.set("");
    this.clearOperation();
    this.operations.set([]);
    this.addValidationToOperationNumber();
  }

  private clearOperation(): void {
    this.operationNumberFormControl.setValue('');
    this.amount.set(0);
    this.selectedProduct.set(undefined);

    //remove the operation number validation
    if (
      this.operationNumberFormControl.hasValidator(OperationNumberValidations.isOperationNumberNotValid)
    ) {
      this.operationNumberFormControl.removeValidators(
        OperationNumberValidations.isOperationNumberNotValid
      );
      this.operationNumberFormControl.updateValueAndValidity({
        onlySelf: true
      });
    }

    this.operationNumberFormControl.reset('');
  }

  async create(): Promise<void> {
    let failed = false;
    for (const operation of this.operations()) {
      operation.plannedStart = new Date().toJSON();
      operation.plannedEnd = new Date().toJSON();
      operation.orderNumber = this.orderNumber();

      this.isLoading.update((_) => true);
      await this.orderManagementService
        .addOperation({sourceId: "Moryx.Orders.Web", body: operation})
        .toAsync()
        .catch(() => {
          failed = true;
          this.isLoading.update((_) => false);
        });
    }
    if (!failed) this.dialog.close();
  }

  onPrimaryClick = async () => {
    await this.performAction(this.primaryAction());
  };

  onSelectAction = (a: Action) => {
    this.primaryAction.set(a);
  };

  isSelected = (a: Action) => this.primaryAction() === a;

  private async performAction(a: Action): Promise<void> {
    const canAdd = this.canAddOperation();

    if (a === Action.AddCreate) {
      if (canAdd) this.addOperation();

      if (this.operations().length > 0) {
        await this.create();
      }
    } else if (canAdd) {
      this.addOperation();
    }
  }
  
  filterProduct(): void {
    const filterValue = this.productInput.nativeElement.value.toLowerCase();
    const filtered = this.possibleProducts().filter(p => this.productToString(p).toLowerCase().includes(filterValue));
    this.filteredProducts.set(filtered);
    if (filtered.length === 1 && filterValue) {
      this.selectedProduct.set(filtered[0]);
    }
  }
  
  filterRecipe(): void {
    const filterValue = this.recipeInput.nativeElement.value.toLowerCase();
    const filtered = this.possibleRecipes().filter(r => this.recipeToString(r).toLowerCase().includes(filterValue));
    this.filteredRecipes.set(filtered);
    if (filtered.length === 1 && filterValue) {
      this.selectedRecipe.set(filtered[0]);
    }
  }

  private async loadRecipes(productIdentifier: string, productRevision: number): Promise<RecipeModel[]> {
    const assignableRecipes = await lastValueFrom(this.orderManagementService
      .getAssignableRecipes({ identifier: productIdentifier, revision: productRevision }));
    
    return await Promise.all(assignableRecipes.map(async (ar) => await this.loadRecipe(ar.id!)));
  }

  private async loadRecipe(id: number): Promise<RecipeModel> {
    return lastValueFrom(this.productManagementService.getRecipe({ id }));
  }

  private byProductNameAndRevision(a: ProductModel, b: ProductModel): number {
    if (!a.name) return 1;
    if (!b.name) return -1;
    if (a.name !== b.name) return a.name.localeCompare(b.name);
    return (b.revision ?? 0) - (a.revision ?? 0);
  }
    
  productToString(value: ProductModel) {
    return value ? `${value.identifier}-${String(value.revision).padStart(2, '0')} ${value.name}` : '';
  }
  
  recipeToString(value: RecipeModel) {
    return value ? `\[${value.type}\] ${value.name}` : '';
  }
}
