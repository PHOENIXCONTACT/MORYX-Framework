/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { EditProductsService } from "../../../services/edit-products.service";
import { map } from 'rxjs';

@Component({
  selector: "app-product-properties",
  templateUrl: "./product-properties.html",
  styleUrls: ["./product-properties.scss"],
  imports: [NavigableEntryEditor]
})
export class ProductProperties {
  private editProductsService = inject(EditProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  properties = toSignal(this.editProductsService.currentProduct$.pipe(map(product => product?.properties)));
}
