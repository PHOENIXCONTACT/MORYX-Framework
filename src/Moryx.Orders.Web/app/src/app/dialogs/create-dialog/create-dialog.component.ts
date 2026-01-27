/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import {
  Component,
  computed,
  effect,
  OnInit,
  signal,
  untracked,
} from "@angular/core";
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormControl,
} from "@angular/forms";
import { MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperationNumberValidations } from "src/app/validations/operationNumberValidations";
import { OrderManagementService } from "../../api/services/order-management.service";
import { ProductManagementService } from "../../api/services/product-management.service";
import { ProductModel } from '../../api/models';
import { RecipeClassificationModel } from '../../api/models';
import { RecipeModel } from '../../api/models';
import { ProductQuery } from '../../api/models';
import { RecipeFilter } from '../../api/models';
import { RevisionFilter } from '../../api/models';
import { OperationCreationContextModel } from '../../api/models';
import { OperationRecipeModel } from '../../api/models';
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatButtonModule } from "@angular/material/button";
import { MatSelectModule } from "@angular/material/select";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatInputModule } from "@angular/material/input";
import { MatMenu, MatMenuModule } from "@angular/material/menu";
import { MatTooltipModule } from "@angular/material/tooltip";

enum Action {
  AddCreate,
  AddOnly
}

@Component({
  selector: "app-create-dialog",
  templateUrl: "./create-dialog.component.html",
  styleUrls: ["./create-dialog.component.scss"],
  standalone: true,
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
    MatTooltipModule
  ],
  providers: [],
})
export class CreateDialogComponent implements OnInit {
  orderNumber = signal("");
  operationNumber = signal("");
  amount = signal(0);
  products = signal<ProductModel[]>([]);
  recipes = signal<RecipeModel[]>([]);
  operations = signal<OperationCreationContextModel[]>([]);
  selectedRecipe = signal<OperationRecipeModel>(<OperationRecipeModel>{});
  isLoading = signal(false);
  selectedProduct = signal<ProductModel | undefined>(undefined);
  canAddOperation = computed(() =>
    this.orderNumber() !== "" &&
    this.operationNumber() !== "" &&
    this.selectedProduct() !== undefined &&
    this.selectedRecipe() !== undefined &&
    this.amount() > 0 &&
    this.operationNumberFormControl.valid
  );
  primaryAction = signal<Action>(Action.AddCreate);
  Action = Action;

  primaryActionLabel = computed(() => {
    return this.primaryAction() === Action.AddCreate
      ? this.translate.instant(TranslationConstants.CREATE_DIALOG.CREATE)
      : this.translate.instant(TranslationConstants.CREATE_DIALOG.ADD);
  });

  canAdd = computed(() => !this.isLoading() && this.canAddOperation());

  canCreate = computed(() => !this.isLoading() && (this.operations().length > 0 || this.canAddOperation()));

  dropdownDisabled = computed(() => !this.canRun(Action.AddCreate) && !this.canRun(Action.AddOnly));

  TranslationConstants = TranslationConstants;
  //Form Controls
  operationNumberFormControl = new UntypedFormControl("", [
    OperationNumberValidations.isOperationNumberNotValid,
  ]);

  async setProduct() {
    const assignableRecipes = await this.orderManagementService
      .getAssignableRecipes({
        identifier: this.selectedProduct()?.identifier!,
        revision: this.selectedProduct()?.revision,
      })
      .toAsync();

    assignableRecipes.forEach(async (assignableRecipe) => {
      const recipe = await this.productManagementService
        .getRecipe({id: assignableRecipe.id!})
        .toAsync();
      this.recipes.update((items) => {
        items.push(recipe);
        return items;
      });

      if (recipe.classification === RecipeClassificationModel.Default) {
        this.selectedRecipe.update((_) => recipe);
      }
    });
  }

  constructor(
    private orderManagementService: OrderManagementService,
    private productManagementService: ProductManagementService,
    private dialog: MatDialogRef<CreateDialogComponent>,
    public translate: TranslateService,
    private snackbarService: SnackbarService
  ) {
    effect(() => {
      const product = this.selectedProduct();
      untracked(() => {
        if (product) {
          this.recipes.set([]);
          this.setProduct().then();
        }
      });
    });
  }

  async ngOnInit(): Promise<void> {
    const query = <ProductQuery>{
      recipeFilter: RecipeFilter.WithRecipe,
      revisionFilter: RevisionFilter.All,
    };
    this.isLoading.update((_) => true);
    await this.productManagementService
      .getTypes({body: query})
      .toAsync()
      // set only products that have a recipe
      .then((value) =>
        this.products.update((_) => value.filter((x) => x.recipes?.length))
      )
      .catch(
        async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      );
    this.isLoading.update((_) => false);
  }

  detailsInputchanged(event: Event | KeyboardEvent) {
    if (this.operations().length > 0) {
      this.addValidationToOperationNumber();
    }
  }

  addOperation(): void {
    const operation = <OperationCreationContextModel>{};
    operation.operationNumber = this.operationNumber();
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
      onlySelf: true,
    });
  }

  fullClear(): void {
    this.orderNumber.set("");
    this.clearOperation();
    this.operations.set([]);
    this.addValidationToOperationNumber();
  }

  private clearOperation(): void {
    this.operationNumber.set("");
    this.selectedProduct.set(undefined);
    this.selectedRecipe.set(<OperationRecipeModel>{});
    this.amount.set(0);
    this.recipes.set([]);

    //remove the operation number validation
    if (
      this.operationNumberFormControl.hasValidator(
        OperationNumberValidations.isOperationNumberNotValid
      ) &&
      this.products.length > 0
    ) {
      this.operationNumberFormControl.removeValidators(
        OperationNumberValidations.isOperationNumberNotValid
      );
      this.operationNumberFormControl.updateValueAndValidity({
        onlySelf: true,
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

  canRun = (a: Action) => (a === Action.AddCreate ? this.canCreate() : this.canAdd());

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
}
