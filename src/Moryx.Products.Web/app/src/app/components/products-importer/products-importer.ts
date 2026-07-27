/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, signal, untracked, ChangeDetectionStrategy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Entry, EntryValueType, NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { ProductImporter } from "@api/models";
import { CacheProductsService } from "@app/services/cache-products.service";

import { MatFormFieldModule } from "@angular/material/form-field";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatInputModule } from "@angular/material/input";
import { MatOptionModule } from "@angular/material/core";
import { MatDividerModule } from "@angular/material/divider";
import { MatIconModule } from "@angular/material/icon";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatSelectModule } from "@angular/material/select";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { toSignal } from "@angular/core/rxjs-interop";
import { map } from "rxjs/operators";
import { MatToolbarModule } from "@angular/material/toolbar";

@Component({
  selector: "app-products-importer",
  templateUrl: "./products-importer.html",
  styleUrls: ["./products-importer.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    TranslatePipe,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatOptionModule,
    MatDividerModule,
    NavigableEntryEditor,
    MatIconModule,
    MatProgressBarModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatToolbarModule
]
})
export class ProductsImporter {
  private cacheService = inject(CacheProductsService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  protected possibleImporters = this.cacheService.importers;
  protected currentImporterName = toSignal(this.route.paramMap.pipe(map((pm) => pm.get("importer"))), {
    initialValue: this.route.snapshot.paramMap.get("importer"),
  });
  protected selectedImporter = computed(() => {
    const name = this.currentImporterName();
    const importers = this.possibleImporters() ?? [];
    return importers.find((i) => i.name === name);
  });
  protected importerProperties = signal<Entry>(<Entry>{value: {type: EntryValueType.Exception}});
  protected showProgressBar = signal(false);

  protected TranslationConstants = TranslationConstants;
  protected Permissions = Permissions;

  constructor() {
    effect(() => {
      const importer = this.selectedImporter();
      untracked(() => {
        if (importer) {
          this.onImporterChanged(importer);
        }
      });
    });
  }

  protected selectImporter(importer: ProductImporter) {
    this.router.navigate(['import', importer.name]);
  }

  private onImporterChanged(importer: ProductImporter) {
    if (importer.parameters !== undefined) {
      this.importerProperties.set(structuredClone(importer.parameters!));
    }
  }

  protected async import() {
    this.showProgressBar.set(true);
    const importer = this.selectedImporter();
    if (!importer?.name) {
      return;
    }
    await this.cacheService.importProducts(importer.name, this.importerProperties());

    this.showProgressBar.set(false);
    this.router.navigate([``]);
  }

  protected cancelImport() {
    this.router.navigate([``]);
  }
}

