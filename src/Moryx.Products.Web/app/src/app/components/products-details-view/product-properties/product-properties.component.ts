/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  effect,
  input,
  OnDestroy,
  OnInit,
  signal,
  untracked,
} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Entry, NavigableEntryEditorComponent } from "@moryx/ngx-web-framework";
import { EditProductsService } from "../../../services/edit-products.service";

import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { firstValueFrom, lastValueFrom, Subscription } from "rxjs";

@Component({
  selector: "app-product-properties",
  templateUrl: "./product-properties.component.html",
  styleUrls: ["./product-properties.component.scss"],
  imports: [
    NavigableEntryEditorComponent,
    MatProgressBarModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
],
  standalone: true,
})
export class ProductPropertiesComponent implements OnInit, OnDestroy {
  properties = signal<Entry | undefined>(undefined);
  id = input.required<number>();
  subscriptions: Subscription[] = [];

  constructor(
    public editService: EditProductsService
  ) {
    effect(() => {
      const id = this.id();
      untracked(async () => {
        await this.initialize(id);
      });
    });
  }

  ngOnDestroy(): void {
    for(let subscription of this.subscriptions){
      subscription.unsubscribe();
    }
  }

  async initialize(id: number) {
    if (id == null) return;
    const subscription = this.editService.currentProduct.subscribe({
      next: product => {
        if (Number(id) === product?.id) {
          this.properties.set( product.properties);
        }
      }
    });
    this.subscriptions.push(subscription);
  }

  ngOnInit(): void {}
}

