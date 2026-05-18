/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { FlatTreeControl } from "@angular/cdk/tree";
import {
  Component,
  computed,
  inject,
  OnDestroy,
  OnInit,
  signal,
  viewChild
} from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { MatDialog } from "@angular/material/dialog";
import { MatMenuModule, MatMenuTrigger } from "@angular/material/menu";
import { MatDrawer, MatSidenavModule } from "@angular/material/sidenav";
import { MatSnackBar } from "@angular/material/snack-bar";
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
  MatTreeModule,
} from "@angular/material/tree";
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from "@angular/router";
import {
  LanguageService,
  SearchBarService,
  SearchRequest,
  SearchSuggestion,
} from "@moryx/ngx-web-framework/services";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { environment } from "src/environments/environment";
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
import { CommonModule } from "@angular/common";
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
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatTooltipModule,
    TranslateModule,
    MatIconModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatMenuModule,
    MatTreeModule,
    RouterOutlet,
    MatInputModule
  ],
  host: {
    '(window:beforeunload)': 'beforeUnloadHander()'
  }
})
export class App implements OnInit, OnDestroy {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private dialog = inject(MatDialog);
  private searchbar = inject(SearchBarService);
  private cacheService = inject(CacheProductsService);
  private editService = inject(EditProductsService);
  private snackBar = inject(MatSnackBar);
  private sessionService = inject(SessionService);
  private languageService = inject(LanguageService);
  private translateService = inject(TranslateService);

  isEditMode = toSignal(this.editService.edit$, { initialValue: false });
  selected = toSignal(this.editService.currentProduct$);
  products = signal<ProductModel[]>([]);
  productDefinitions = signal<ProductDefinitionModel[]>([]);
  hierarchic = signal(false);
  revisionOptions = signal<string[]>(Object.keys(RevisionFilter));
  selectorOptions = signal<string[]>(Object.keys(Selector));
  importers = toSignal(this.cacheService.importers$, { initialValue: [] });
  menuTopLeftPosition = signal<{ x: String, y: String }>({x: '0', y: '0'});
  trigger = viewChild.required(MatMenuTrigger);

  TranslationConstants = TranslationConstants;

  title = "Moryx.Products.Web";
  productsToolbarImage: string =
    environment.assets + "assets/products_toolbar.jpg";

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translateService.setFallbackLang("en");
    this.translateService.use(this.languageService.getDefaultLanguage());
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translateService
      .get([TranslationConstants.APP.SNACK_BAR])
      .toAsync();
  }

  ngOnDestroy(): void {
    this.searchbar.unsubscribe();
  }

  async ngOnInit() {
    this.hierarchic.set(this.sessionService.getProductTreeHierarchy());

    this.cacheService.productsShownInTheTree.subscribe((products) => {
      this.products.set(products ?? []);
      this.createDatasource(this.hierarchic());
    });

    this.cacheService.definitions.subscribe((definitions) => {
      this.productDefinitions.set(definitions ?? []);
      this.createDatasource(this.hierarchic());
    });

    // ToDo: MOve to route resolver for base path
    this.cacheService.loadConfiguration();
    this.cacheService.loadProductsForTree();

    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  onSearch(result: SearchRequest) {
    if (!this.products().length) return;

    const searchterm = result.term;
    let products = this.products().filter((p) =>
      this.editService.createProductNameWithIdentity(p).includes(searchterm)
    );
    if (!products.length) products = [];
    if (result.submitted) {
      this.searchbar.clearSuggestions();
      if (products.length > 1)
        this.router.navigate(["search"], {queryParams: {q: searchterm}});
      else if (products.length === 1)
        this.router.navigate(['/details', products[0].id ?? 0]);
      this.searchbar.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        }
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (let product of products) {
        //TODO: change this in MORYX 12
        const url = "Products/details/" + product.id; // <= BAD, hard coding a parent url 'Products' is no reliable.
        if (!product.id) continue;

        searchSuggestions.push({
          text: this.editService.createProductNameWithIdentity(product),
          url: url
        });
      }

      this.searchbar.provideSuggestions(searchSuggestions);
    }
  }

  private _transformer = (node: ProductNode, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
      id: node.id,
      identifier: node.identifier,
      revision: node.revision,
    } as FlatNode;
  };

  treeControl = new FlatTreeControl<FlatNode>(
    (node) => node.level,
    (node) => node.expandable
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    (node) => node.level,
    (node) => node.expandable,
    (node) => node.children
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  hasChild = (_: number, node: FlatNode) => node.expandable;

  createDatasource(hierarchic: boolean) {
    if (this.productDefinitions().length === 0) return;

    this.hierarchic.set(hierarchic);

    let dataSource = [] as ProductNode[];
    if (!hierarchic) {
      dataSource = this.SortTypesToDefinitions();
    } else {
      const products = this.SortTypesToDefinitions();
      for (let p of products) {
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
    this.dataSource.data = dataSource;
    this.sessionService.expandNodesAccordingToStorage(this.treeControl);
  }

  beforeUnloadHander() {
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

  saveDisabled(): boolean {
    const anyUnsetRecipes = this.selected()?.recipes?.some((r) => r.classification === RecipeClassificationModel.Unset);

    if (this.isEditMode() && this.selected() && anyUnsetRecipes) {
      return true;
    }
    return false;
  }

  setHierarchy(hierarchy: boolean) {
    this.sessionService.setProductTreeHierarchy(hierarchy);
    this.createDatasource(hierarchy);
  }

  onExpandOrCollapseNode(node: FlatNode) {
    this.sessionService.saveProductTreeExpansion(
      node,
      this.treeControl.isExpanded(node)
    );
  }

  private SortTypesToDefinitions(): ProductNode[] {
    if (this.productDefinitions().length === 0) return [];
    let products = [] as ProductNode[];
    for (let definition of this.productDefinitions()) {
      const d = this.ConvertTypeDefinitionToNode(definition);
      const types = this.products().filter((p) => p.type === definition.name);
      if (types) {
        for (let type of types) {
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

  onProductContext(event: MouseEvent, productId: number) {
    // Only handle right-click, not touch long-press
    if ((event as any).pointerType === 'touch') {
      return;
    }
    event.preventDefault();
    if (productId === 0) return;

    this.open(event.clientX, event.clientY, productId);
  }

  private open(x: number, y: number, productId: number) {
    this.trigger().menuData = {id: productId};
    this.menuTopLeftPosition.update(_ => {
      return {x: `${x}px`, y: `${y}px`}
    });
    this.trigger().openMenu();
  }

  async onDeselect() {    
    if (this.isEditMode()) {
      await this.editService.onCancel();
    }
    this.editService.resetProduct();
    await this.router.navigate([``]);
  }

  onSelect(id: number) {
    if (this.isEditMode()) return;

    if (id == 0) return;

    if (id === this.selected()?.id) return;

    this.router.navigate(['/details', id]);
  }

  async clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName !== "MAT-TREE") {
      return;
    }
    this.onDeselect();
  }

  onDelete(id: number | undefined) {
    if (!id) return;
    const product = this.products().find((p) => p.id == id);
    if (!product) return;

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

  async onImport() {
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

  onEdit() {
    this.searchbar.clearSuggestions();
    this.searchbar.unsubscribe();
    this.editService.onEdit();
  }

  async onCancel() {
    await this.editService.onCancel();
    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  onSave() {
    this.editService.onSave();
    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      }
    });
  }

  onSelectAndEdit(id: number) {
    this.searchbar.clearSuggestions();
    this.searchbar.unsubscribe();

    if (this.editService.currentProductId() === id) {
      this.editService.onEdit();
      return;
    }

    this.router.navigate(['/details', id])
      .then(() => this.editService.onEdit());
  }

  onDuplicate(id: number | undefined) {
    if (!id) return;

    const product = this.products().find((p) => p.id == id);
    if (!product) return;

    const dialogRef = this.dialog.open(DialogDuplicateProduct, {
      data: product
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.editService.onDuplicate(result as DuplicateProductInfos);
    });
  }

  onRevisions(id: number | undefined) {
    if (!id) return;

    const product = this.products().find((p) => p.id == id);
    if (!product) return;

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

  resetFilter(drawer: MatDrawer) {
    this.cacheService.resetFilter();
    drawer.toggle();
  }

  filter(drawer: MatDrawer) {
    this.cacheService.loadProductsForTree();
    drawer.toggle();
  }

  createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editService.createProductIdentity(identifier, revision);
  }

  get filterOptions() {
    return this.cacheService.filterOptions;
  }

  refreshProducts(): void {
    this.cacheService.loadProductsForTree();
  }
}

export interface FlatNode {
  expandable: boolean;
  name: string;
  level: number;
  id: number;
  identifier: string;
  revision: number;
}

interface ProductNode {
  name: string;
  typeName: string | undefined;
  baseType: string | undefined;
  id: number;
  children?: ProductNode[];
  identifier: string;
  revision: number;
}

