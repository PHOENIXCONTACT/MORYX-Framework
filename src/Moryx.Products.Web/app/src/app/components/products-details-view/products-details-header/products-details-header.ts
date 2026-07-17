/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, linkedSignal, ChangeDetectionStrategy } from "@angular/core";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { EditProductsService } from "@app/services/edit-products.service";
import { ProductModel, ProductState } from "../../../api/models";

import { MatInputModule } from "@angular/material/input";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatOptionModule } from "@angular/material/core";
import { MatDividerModule } from "@angular/material/divider";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";

@Component({
  selector: "app-products-details-header",
  templateUrl: "./products-details-header.html",
  styleUrls: ["./products-details-header.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatInputModule,
    TranslatePipe,
    ReactiveFormsModule,
    FormsModule,
    MatOptionModule,
    MatDividerModule,
    MatFormFieldModule,
    MatSelectModule
  ]
})
export class ProductsDetailsHeader {
  private editService = inject(EditProductsService);

  protected currentProduct = this.editService.currentProduct;
  protected editMode = this.editService.editing;
  protected identifier = linkedSignal(() => {
    const current = this.currentProduct();
    if (!current) {
      return;
    }
    return this.editService.createProductIdentity(current.identifier, current.revision);
  });
  protected possibleStates = signal<string[]>(Object.values(ProductState));

  protected TranslationConstants = TranslationConstants;

  protected updateCurrentProduct(patch: Partial<ProductModel>) {
    const current = this.currentProduct();
    if (!current) {
      return;
    }

    this.editService.updateCurrentProduct({ ...current, ...patch });
  }
}
