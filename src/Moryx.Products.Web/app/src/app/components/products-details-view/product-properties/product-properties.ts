/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, ChangeDetectionStrategy } from "@angular/core";
import { NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { EditProductsService } from "@app/services/edit-products.service";

@Component({
  selector: "app-product-properties",
  templateUrl: "./product-properties.html",
  styleUrls: ["./product-properties.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [NavigableEntryEditor]
})
export class ProductProperties {
  private editProductsService = inject(EditProductsService);

  protected isEditMode = this.editProductsService.editing;
  protected properties = computed(() => this.editProductsService.currentProduct()?.properties);
}
