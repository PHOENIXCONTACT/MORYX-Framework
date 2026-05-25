/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, signal, linkedSignal } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { EditProductsService } from "src/app/services/edit-products.service";
import { ProductModel, ProductState } from "../../../api/models";

import { MatInputModule } from "@angular/material/input";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatOptionModule } from "@angular/material/core";
import { MatDividerModule } from "@angular/material/divider";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { toSignal } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-products-details-header",
  templateUrl: "./products-details-header.html",
  styleUrls: ["./products-details-header.scss"],
  imports: [
    MatInputModule,
    TranslateModule,
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
  
  currentProduct = toSignal(this.editService.currentProduct$);
  editMode = toSignal(this.editService.edit$, { initialValue: false });
  identifier = linkedSignal(() => {
    const current = this.currentProduct();
    if (!current) {
      return;
    }
    return this.editService.createProductIdentity(current.identifier, current.revision);
  });
  possibleStates = signal<string[]>(Object.values(ProductState));

  TranslationConstants = TranslationConstants;

  updateCurrentProduct(patch: Partial<ProductModel>) {
    const current = this.currentProduct();
    if (!current) {
      return;
    }

    this.editService.updateCurrentProduct({ ...current, ...patch });
  }
}
