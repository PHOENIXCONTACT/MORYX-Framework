/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, input, Input, OnInit, signal } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { EditProductsService } from "src/app/services/edit-products.service";
import { ProductModel, ProductState } from "../../../api/models";

import { MatInputModule } from "@angular/material/input";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatOptionModule } from "@angular/material/core";
import { MatDividerModule } from "@angular/material/divider";
import { untracked } from "@angular/core/primitives/signals";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
@Component({
  selector: "app-products-details-header",
  templateUrl: "./products-details-header.component.html",
  styleUrls: ["./products-details-header.component.scss"],
  standalone: true,
  imports: [
    MatInputModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    MatOptionModule,
    MatDividerModule,
    MatFormFieldModule,
    MatSelectModule
],
})
export class ProductsDetailsHeaderComponent implements OnInit {
  currentProduct = input.required<ProductModel>();
  identifier = signal<string | undefined>(undefined);
  editMode = input.required<boolean>();
  possibleStates = signal<string[]>(Object.values(ProductState));

  TranslationConstants = TranslationConstants;

  constructor(
    public editService: EditProductsService,
    public dialog: MatDialog,
    public translate: TranslateService
  ) {
    effect(() => {
      const product = this.currentProduct();
      untracked(() => {
        if (!product || product.revision === undefined) return;
        if (product.identifier)
          this.identifier.update((_) =>
            this.editService.createProductIdentity(
              product.identifier,
              product.revision
            )
          );
      });
    });
  }

  ngOnInit(): void {}
}

