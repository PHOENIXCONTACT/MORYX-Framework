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
import { MoryxSnackbarService } from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperationNumberValidations } from "src/app/validations/operationNumberValidations";
import { OrderManagementService } from "../../api/services/order-management.service";
import { ProductManagementService } from "../../api/services/product-management.service";
import { ProductModel } from "src/app/api/models/Moryx/AbstractionLayer/Products/Endpoints/product-model";
import { RecipeClassificationModel } from "src/app/api/models/Moryx/AbstractionLayer/Products/Endpoints/recipe-classification-model";
import { RecipeModel } from "src/app/api/models/Moryx/AbstractionLayer/Products/Endpoints/recipe-model";
import { ProductQuery } from "src/app/api/models/Moryx/AbstractionLayer/Products/product-query";
import { RecipeFilter } from "src/app/api/models/Moryx/AbstractionLayer/Products/recipe-filter";
import { RevisionFilter } from "src/app/api/models/Moryx/AbstractionLayer/Products/revision-filter";
import { OperationCreationContextModel } from "src/app/api/models/Moryx/Orders/Endpoints/Models/operation-creation-context-model";
import { OperationRecipeModel } from "src/app/api/models/Moryx/Orders/Endpoints/operation-recipe-model";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatButtonModule } from "@angular/material/button";
import { MatSelectModule } from "@angular/material/select";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatInputModule } from "@angular/material/input";

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
    MatInputModule
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
        .getRecipe({ id: assignableRecipe.id! })
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
    private moryxSnackbar: MoryxSnackbarService
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
      .getTypes({ body: query })
      .toAsync()
      // set only products that have a recipe
      .then((value) =>
        this.products.update((_) => value.filter((x) => x.recipes?.length))
      )
      .catch(
        async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e)
      );
    this.isLoading.update((_) => false);
  }

  detailsInputchanged(event: Event | KeyboardEvent) {
    if (this.operations.length > 0) {
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
    if (this.operations.length === 0) this.addValidationToOperationNumber();
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
  }

  async create(): Promise<void> {
    let failed = false;
    for (const operation of this.operations()) {
      operation.plannedStart = new Date().toJSON();
      operation.plannedEnd = new Date().toJSON();
      operation.orderNumber = this.orderNumber();

      this.isLoading.update((_) => true);
      await this.orderManagementService
        .addOperation({ sourceId: "Moryx.Orders.Web", body: operation })
        .toAsync()
        .catch(() => {
          failed = true;
          this.isLoading.update((_) => false);
        });
    }
    if (!failed) this.dialog.close();
  }
}
