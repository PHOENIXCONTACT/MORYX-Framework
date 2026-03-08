/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  effect,
  inject,
  input,
  OnDestroy,
  signal,
  untracked,
} from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { Entry, NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { EditProductsService } from "../../../services/edit-products.service";

import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { Subscription } from 'rxjs';

@Component({
  selector: "app-product-properties",
  templateUrl: "./product-properties.html",
  styleUrls: ["./product-properties.scss"],
  imports: [
    NavigableEntryEditor,
    MatProgressBarModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
  ]
})
export class ProductProperties implements OnDestroy {
  private editProductsService = inject(EditProductsService);

  isEditMode = toSignal(this.editProductsService.edit$, { initialValue: false });
  properties = signal<Entry | undefined>(undefined);
  id = input.required<number>();
  subscriptions: Subscription[] = [];

  constructor() {
    effect(() => {
      const id = this.id();
      untracked(async () => {
        await this.initialize(id);
      });
    });
  }

  ngOnDestroy(): void {
    for (let subscription of this.subscriptions) {
      subscription.unsubscribe();
    }
  }

  async initialize(id: number) {
    if (id == null) return;
    const subscription = this.editProductsService.currentProduct.subscribe({
      next: product => {
        if (Number(id) === product?.id) {
          this.properties.set(product.properties);
        }
      }
    });
    this.subscriptions.push(subscription);
  }
}

