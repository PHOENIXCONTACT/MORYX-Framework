/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {
  Component,
  effect,
  inject,
  OnDestroy,
  OnInit,
  signal,
  untracked,
  viewChild,
  ChangeDetectionStrategy
} from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatMenuModule, MatMenuTrigger } from "@angular/material/menu";
import { MatDrawer, MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ProductTree } from "./components/product-tree/product-tree";
import { Router, RouterOutlet } from "@angular/router";
import {
  LanguageService,
  SearchBarService,
  SearchRequest,
  SearchSuggestion,
} from "@moryx/ngx-web-framework/services";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { firstValueFrom } from "rxjs";
import { environment } from "../environments/environment";
import {
  ProductDefinitionModel,
  ProductModel,
  RecipeClassificationModel, RevisionFilter,
  Selector,
} from "./api/models";
import { DialogCreateRevision } from "./dialogs/dialog-create-revision/dialog-create-revision";
import { DialogDuplicateProduct } from "./dialogs/dialog-duplicate-product/dialog-duplicate-product";
import { DialogRemoveProduct } from "./dialogs/dialog-remove-product/dialog-remove-product";
import { DialogShowRevisions } from "./dialogs/dialog-show-revisions/dialog-show-revisions";
import { TranslationConstants } from "./extensions/translation-constants.extensions";
import { DuplicateProductInfos } from "./models/DuplicateProductInfos";
import { CacheProductsService } from "./services/cache-products.service";
import { EditProductsService } from "./services/edit-products.service";
import {
  ProductStorageDetails,
  SessionService,
} from "./services/session.service";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatButtonModule } from "@angular/material/button";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatIconModule } from "@angular/material/icon";
import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: "app-root",
  templateUrl: "./app.html",
  styleUrls: ["./app.scss"],
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatTooltipModule,
    TranslatePipe,
    MatIconModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatMenuModule,
    ProductTree,
    RouterOutlet,
    MatInputModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  host: {
    '(window:beforeunload)': 'beforeUnloadHander()'
  }
})
export class App implements OnInit, OnDestroy {
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private searchbar = inject(SearchBarService);
  private cacheService = inject(CacheProductsService);
  private editService = inject(EditProductsService);
  private snackBar = inject(MatSnackBar);
  private sessionService = inject(SessionService);
  private languageService = inject(LanguageService);
  private translateService = inject(TranslateService);

  protected isEditMode = this.editService.editing;
  protected selected = this.editService.currentProduct;
  protected products = signal<ProductModel[]>([]);
  protected productDefinitions = signal<ProductDefinitionModel[]>([]);
  protected hierarchic = signal(false);
  protected revisionOptions = signal<string[]>(Object.keys(RevisionFilter));
  protected selectorOptions = signal<string[]>(Object.keys(Selector));
  protected importers = this.cacheService.importers;
  protected menuTopLeftPosition = signal<{ x: string, y: string }>({x: '0', y: '0'});
  protected readonly trigger = viewChild.required(MatMenuTrigger);

  protected TranslationConstants = TranslationConstants;

  protected productsToolbarImage: string =
    environment.assets + "assets/products_toolbar.jpg";

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translateService.setFallbackLang("en");
    this.translateService.use(this.languageService.getFallbackLang());

    effect(() => {
      const products = this.cacheService.productsShownInTheTree() ?? [];
      untracked(() => {
        this.products.set(products);
        this.createDatasource(this.hierarchic());
      });
    });

    effect(() => {
      const definitions = this.cacheService.definitions() ?? [];
      untracked(() => {
        this.productDefinitions.set(definitions);
        this.createDatasource(this.hierarchic());
      });
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([TranslationConstants.APP.SNACK_BAR]));
  }

  ngOnDestroy(): void {
    this.searchbar.unsubscribe();
  }

  async ngOnInit() {
    this.hierarchic.set(this.sessionService.getProductTreeHierarchy());

    // ToDo: MOve to route resolver for base path
    this.cacheService.loadConfiguration();
    this.cacheService.loadProductsForTree();

    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  private onSearch(result: SearchRequest) {
    if (!this.products().length) {
      return;
    }

    const searchTerm = result.term;
    let products = this.products().filter((p) =>
      this.editService.createProductNameWithIdentity(p).includes(searchTerm)
    );
    if (!products.length) {
      products = [];
    }
    if (result.submitted) {
      this.searchbar.clearSuggestions();
      if (products.length > 1) {
        this.router.navigate(["search"], {queryParams: {q: searchTerm}});
      }
      else if (products.length === 1) {
        this.router.navigate(['/details', products[0].id ?? 0]);
      }
      this.searchbar.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        }
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (const product of products) {
        //TODO: change this in MORYX 12
        const url = "Products/details/" + product.id; // <= BAD, hard coding a parent url 'Products' is no reliable.
        if (!product.id) {
          continue;
        }

        searchSuggestions.push({
          text: this.editService.createProductNameWithIdentity(product),
          url: url
        });
      }

      this.searchbar.provideSuggestions(searchSuggestions);
    }
  }

  protected treeData = signal<ProductNode[]>([]);

  private createDatasource(hierarchic: boolean) {
    if (this.productDefinitions().length === 0) {
      return;
    }

    this.hierarchic.set(hierarchic);

    let dataSource = [] as ProductNode[];
    if (!hierarchic) {
      dataSource = this.SortTypesToDefinitions();
    } else {
      const products = this.SortTypesToDefinitions();
      for (const p of products) {
        if (p.baseType && p.baseType !== "ProductType") {
          //check for parent in configured types
          let parent = products.find((e) => e.typeName === p.baseType);
          if (!parent) {
            parent = dataSource.find((e) => e.typeName === p.baseType);
            //create parent if not found
            if (!parent) {
              parent = {
                name: p.baseType,
                typeName: p.baseType,
                id: 0,
                children: [] as ProductNode[],
              } as ProductNode;
              dataSource.push(parent);
            }
          }
          parent.children?.push(p);
        } else {
          dataSource.push(p);
        }
      }
    }
    this.treeData.set(dataSource);
  }

  protected beforeUnloadHander() {
    const product = this.selected();
    if (this.isEditMode() && product) {
      this.sessionService.pushWipProduct(product, <ProductStorageDetails>{
        currentPartId: this.editService.currentPartId,
        currentRecipeNumber: this.editService.currentRecipeNumber,
        maximumAlreadySavedPartId: this.editService.maximumAlreadySavedPartId,
        maximumAlreadySavedRecipeId: this.editService.maximumAlreadySavedRecipeId
      });
    }
  }

  protected saveDisabled(): boolean {
    const anyUnsetRecipes = this.selected()?.recipes?.some((r) => r.classification === RecipeClassificationModel.Unset);

    if (this.isEditMode() && this.selected() && anyUnsetRecipes) {
      return true;
    }
    return false;
  }

  protected setHierarchy(hierarchy: boolean) {
    this.sessionService.setProductTreeHierarchy(hierarchy);
    this.createDatasource(hierarchy);
  }

  private SortTypesToDefinitions(): ProductNode[] {
    if (this.productDefinitions().length === 0) {
      return [];
    }
    const products = [] as ProductNode[];
    for (const definition of this.productDefinitions()) {
      const d = this.ConvertTypeDefinitionToNode(definition);
      const types = this.products().filter((p) => p.type === definition.name);
      if (types) {
        for (const type of types) {
          const t = this.ConvertTypeToNode(type);
          d.children?.push(t);
        }
      }
      products.push(d);
    }
    return products;
  }

  private ConvertTypeDefinitionToNode(
    model: ProductDefinitionModel
  ): ProductNode {
    return {
      name: model.displayName,
      typeName: model.name,
      id: 0,
      children: [] as ProductNode[],
      baseType: model.baseDefinition,
    } as ProductNode;
  }

  private ConvertTypeToNode(model: ProductModel): ProductNode {
    return {
      name: model.name,
      id: model.id,
      identifier: model.identifier,
      revision: model.revision,
    } as ProductNode;
  }

  protected onProductContext(event: MouseEvent, productId: number) {
    // Only handle right-click, not touch long-press
    if ((event as PointerEvent).pointerType === 'touch') {
      return;
    }
    event.preventDefault();
    if (productId === 0) {
      return;
    }

    this.open(event.clientX, event.clientY, productId);
  }

  private open(x: number, y: number, productId: number) {
    this.trigger().menuData = {id: productId};
    this.menuTopLeftPosition.set({x: `${x}px`, y: `${y}px`});
    this.trigger().openMenu();
  }

  protected async onDeselect() {
    if (this.isEditMode()) {
      await this.editService.onCancel();
    }
    this.editService.resetProduct();
    await this.router.navigate([``]);
  }

  protected onSelect(id: number) {
    if (this.isEditMode()) {
      return;
    }

    if (id == 0) {
      return;
    }

    if (id === this.selected()?.id) {
      return;
    }

    this.router.navigate(['/details', id]);
  }

  protected async clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName !== "MAT-TREE") {
      return;
    }
    this.onDeselect();
  }

  protected onDelete(id: number | undefined) {
    if (!id) {
      return;
    }
    const product = this.products().find((p) => p.id == id);
    if (!product) {
      return;
    }

    const dialogRef = this.dialog.open(DialogRemoveProduct, {
      data: product
    });

    dialogRef.afterClosed().subscribe(async (productToBeDeleted) => {
      if (productToBeDeleted) {
        const actualProduct = productToBeDeleted();
        await this.cacheService.deleteProduct(actualProduct);
        this.editService.resetProduct();
      }
    });
  }

  protected async onImport() {
    const importers = this.importers();
    const target = importers?.length ? importers[0].name : undefined;
    if (target) {
      this.router.navigate(['import', target]);
    } else {
      const translations = await this.getTranslations();
      this.snackBar.open(
        translations[TranslationConstants.APP.SNACK_BAR],
        "x",
        {
          horizontalPosition: "center",
          verticalPosition: "top",
        }
      );
    }
  }

  protected onEdit() {
    this.searchbar.clearSuggestions();
    this.searchbar.unsubscribe();
    this.editService.onEdit();
  }

  protected async onCancel() {
    await this.editService.onCancel();
    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  protected onSave() {
    this.editService.onSave();
    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  protected onSelectAndEdit(id: number) {
    this.searchbar.clearSuggestions();
    this.searchbar.unsubscribe();

    if (this.editService.currentProductId() === id) {
      this.editService.onEdit();
      return;
    }

    this.router.navigate(['/details', id])
      .then(() => this.editService.onEdit());
  }

  protected onDuplicate(id: number | undefined) {
    if (!id) {
      return;
    }

    const product = this.products().find((p) => p.id == id);
    if (!product) {
      return;
    }

    const dialogRef = this.dialog.open(DialogDuplicateProduct, {
      data: product
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.editService.onDuplicate(result as DuplicateProductInfos);
      }
    });
  }

  protected onRevisions(id: number | undefined) {
    if (!id) {
      return;
    }

    const product = this.products().find((p) => p.id == id);
    if (!product) {
      return;
    }

    const dialogRef = this.dialog.open(DialogShowRevisions, {
      data: product
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.dialog.open(DialogCreateRevision, {
          data: product
        });
      }
    });
  }

  protected resetFilter(drawer: MatDrawer) {
    this.cacheService.resetFilter();
    drawer.toggle();
  }

  protected filter(drawer: MatDrawer) {
    this.cacheService.loadProductsForTree();
    drawer.toggle();
  }

  protected get filterOptions() {
    return this.cacheService.filterOptions;
  }

  protected refreshProducts(): void {
    this.cacheService.loadProductsForTree();
  }
}

export interface ProductNode {
  name: string;
  typeName: string | undefined;
  baseType: string | undefined;
  id: number;
  children?: ProductNode[];
  identifier: string;
  revision: number;
}

