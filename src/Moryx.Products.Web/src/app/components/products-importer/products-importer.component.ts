import { Component, effect, OnInit, signal, untracked } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import {
  Entry,
  EntryValueType,
  NavigableEntryEditorComponent,
  PermissionService,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { ProductImporter } from "../../api/models";
import { CacheProductsService } from "../../services/cache-products.service";
import { Permissions } from "./../../extensions/permissions.extensions";
import { CommonModule } from "@angular/common";
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

@Component({
  selector: "app-products-importer",
  templateUrl: "./products-importer.component.html",
  styleUrls: ["./products-importer.component.scss"],
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    MatOptionModule,
    MatDividerModule,
    NavigableEntryEditorComponent,
    MatIconModule,
    MatProgressBarModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
  ],
})
export class ProductsImporterComponent implements OnInit {
  possibleImporters = signal<ProductImporter[]>([]);
  selectedImporter = signal<ProductImporter | undefined>(undefined);
  importerProperties = signal<Entry>(<Entry>{ value: { type: EntryValueType.Exception} });
  showProgressBar = signal(false);

  TranslationConstants = TranslationConstants;
  Permissions = Permissions;
  constructor(
    public cacheService: CacheProductsService,
    private route: ActivatedRoute,
    private router: Router,
    public translate: TranslateService,
  ) {
    this.cacheService.importers.subscribe((importers) => {
      if (importers) {
        this.possibleImporters.update((_) => importers);
        const importerName = this.route.snapshot.paramMap.get("importer");
        if (!importerName) return;

        this.selectedImporter.update((_) =>
          this.possibleImporters()?.find((i) => i.name === importerName)
        );
      }
    });

    effect(() => {
      const importer = this.selectedImporter();
      untracked(() => {
        if (importer) {
          this.onImporterChanged(importer);
        }
      });
    });
  }

  ngOnInit(): void {}

  onImporterChanged(importer: ProductImporter) {
    if (importer.parameters !== undefined) {
      this.importerProperties.update((_) =>
        this.cacheService.cloneEntry(importer.parameters!)
      );
    }
  }

  import() {
    this.showProgressBar.update((_) => true);
    if (this.selectedImporter() && this.selectedImporter()?.name)
      this.cacheService
        .importProducts(this.selectedImporter()?.name!, this.importerProperties())
        .then((resolved) => {
          //refresh/go to default hom page
          if (resolved) {
            this.showProgressBar.update((_) => false);
            this.router.navigate([``]);
          }
        });
  }

  cancelImport() {
    this.router.navigate([``]);
  }
}
