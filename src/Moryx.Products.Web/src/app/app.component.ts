import { FlatTreeControl } from "@angular/cdk/tree";
import {
  Component,
  HostListener,
  OnDestroy,
  OnInit,
  signal,
  viewChild,
  ViewChild,
} from "@angular/core";
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
  PermissionService,
  SearchBarService,
  SearchRequest,
  SearchSuggestion,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { environment } from "src/environments/environment";
import {
  ProductDefinitionModel,
  ProductModel,
  RecipeClassificationModel,
  RevisionFilter,
  Selector,
} from "./api/models";
import { DialogCreateRevisionComponent } from "./dialogs/dialog-create-revision/dialog-create-revision.component";
import { DialogDuplicateProductComponent } from "./dialogs/dialog-duplicate-product/dialog-duplicate-product.component";
import { DialogRemoveProductComponent } from "./dialogs/dialog-remove-product/dialog-remove-product.component";
import { DialogShowRevisionsComponent } from "./dialogs/dialog-show-revisions/dialog-show-revisions.component";
import { Permissions } from "./extensions/permissions.extensions";
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
    templateUrl: "./app.component.html",
    styleUrls: ["./app.component.scss"],
    standalone: true,
    imports:[ 
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
    ]
})
export class AppComponent implements OnInit, OnDestroy {
  selected = signal<ProductModel| undefined>(undefined);
  products = signal<ProductModel[]>([]);
  productDefinitions = signal<ProductDefinitionModel[]>([]);
  hierarchic = signal(false);
  revisionOptions = signal<string[]>(Object.keys(RevisionFilter));
  selectorOptions = signal<string[]>(Object.keys(Selector));
  importer = signal<string | undefined>(undefined);
  menuTopLeftPosition = signal<{ x: String, y: String }>({x: '0', y: '0'});
  trigger = viewChild.required(MatMenuTrigger);

  TranslationConstants = TranslationConstants;
  Permissions = Permissions;

  private userPermissions = signal<string[]>([]);
  private ignoreIam = environment.ignoreIam;

  title = "Moryx.Products.Web";
  productsToolbarImage: string =
    environment.assets + "assets/products_toolbar.jpg";
  
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    public dialog: MatDialog,
    private searchbar: SearchBarService,
    public cacheService: CacheProductsService,
    public editService: EditProductsService,
    private snackBar: MatSnackBar,
    private sessionService: SessionService,
    private languageService: LanguageService,
    private permissionService: PermissionService,
    public translate: TranslateService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translate.setDefaultLang("en");
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
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

    this.cacheService.importers.subscribe((importers) => {
      if (importers && importers.length > 0 && importers[0].name)
        this.importer.set(importers[0].name!);
    });

    this.editService.currentProduct.subscribe((product) => {
      this.selected.set(product);
    });

    this.router.events.subscribe((e) => {
      if (e instanceof NavigationEnd) this.selectCurrentProduct();
    });

    this.cacheService.loadConfiguration();
    this.cacheService.loadProductsForTree();

    const wipProduct = this.sessionService.getWipProduct();
    if (wipProduct) {
      this.editService.loadFromStorage();
    } else {
      this.editService.loadProduct();
    }

    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      },
    });

    if (!this.ignoreIam)
    {
      var permissions = await this.permissionService.getPermissions();
      this.userPermissions.set( permissions);
    }
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
      if(products.length > 1)
        this.router.navigate(["search"], { queryParams: { q: searchterm } });
      else if (products.length === 1)
        this.routeToAnotherProductOnSelect(products[0].id ?? 0);
      this.searchbar.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (let product of products) {
        //TODO: change this in MORYX 10
        const url = "Products/details/" + product.id; // <= BAD, hard coding a parent url 'Products' is no reliable. 
        if (!product.id) continue;

        searchSuggestions.push({
          text: this.editService.createProductNameWithIdentity(product),
          url: url,
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

    this.hierarchic.set( hierarchic);

    let dataSource = [] as ProductNode[];
    if (hierarchic === false) {
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

  @HostListener("window:beforeunload")
  beforeUnloadHander() {
    if (this.editService.edit && this.selected()) {
      this.sessionService.setWipProduct(this.selected()!, <ProductStorageDetails>{
        currentPartId: this.editService.currentPartId,
        currentRecipeNumber: this.editService.currentRecipeNumber,
        maximumAlreadySavedPartId: this.editService.maximumAlreadySavedPartId,
        maximumAlreadySavedRecipeId:
          this.editService.maximumAlreadySavedRecipeId,
      });
    }
  }

  hasPermission(permission: string) {
    if (environment.ignoreIam) {
      return true;
    }

    if (window.configs && !window.configs.identityUrl) {
      return true;
    }

    return this.userPermissions().any((p) => p === permission);
  }

  importDisabled(): boolean {
    return (
      this.editService.edit || !this.hasPermission(Permissions.CAN_IMPORT_REVISIONS)
    );
  }

  editDisabled() {
    if (!this.selected()) {
      return true;
    }

    return !this.hasPermission(this.Permissions.CAN_EDIT_PRODUCT);
  }

  saveDisabled(): boolean {
    if (
      this.editService.edit &&
      this.selected() &&
      this.selected()?.recipes?.any(
        (r) => r.classification === RecipeClassificationModel.Unset
      )
    ) {
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
    event.preventDefault();
    if (productId === 0) return;
    
    this.open(event.clientX, event.clientY, productId); 
  }

  private open(x: number, y: number, productId: number){
    this.trigger().menuData = { id: productId };
    this.menuTopLeftPosition.update(_ => {
      return { x : `${x}px`, y: `${y}px`}
    });
    this.trigger().openMenu();
  }

  async onDeselect() {
    if (this.editService.edit)
      await this.onCancel();
    this.editService.unloadProduct();
  }

  onSelect(id: number) {
    if (this.editService.edit) return;

    if (id == 0) return;

    if (id === this.selected()?.id) return;

    const url = this.router.url;
    const regexSpecificRecipe: RegExp = /(details\/\d*\/recipes\/\d*)/;
    const regexParts: RegExp = /(details\/\d*\/parts)/;
    if (regexSpecificRecipe.test(url) || regexParts.test(url)) {
      this.router.navigate(["../../"], { relativeTo: this.route }).then(() => {
        this.routeToAnotherProductOnSelect(id);
      });
    } else {
      this.routeToAnotherProductOnSelect(id);
    }
  }

  onOpenContextMenu(event: any, id: number) {
    this.open(event.pointers[0].clientX, event.pointers[0].clientY, id);
  }

  private routeToAnotherProductOnSelect(id: number) {
    const product = this.products().find((p) => p.id === id);
    if (product) {
      this.router
        .navigate([`details/${product.id}`], { relativeTo: this.route })
        .then(() => this.editService.loadProduct());
    } else this.router.navigate([``]);
  }

  selectCurrentProduct() {
    const url = this.router.url;
    const regexId: RegExp = /(details\/\d*)/;
    if (!regexId.test(url)) {
      this.selected.set(undefined);
      return;
    }

    const id = Number(url.split("/")[2]);
    if (this.selected()?.id != id)
      this.selected.set(this.products()?.find((p) => p.id === id));
  }

  clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName === "MAT-TREE") {
      this.snackBar.dismiss();
      this.router
        .navigate([``], { relativeTo: this.route })
        .then(() => this.editService.onCancel());
    }
  }

  onDelete(id: number | undefined) {
    if (!id) return;
    const product = this.products().find((p) => p.id == id);
    if (!product) return;
    
    const dialogRef = this.dialog.open(DialogRemoveProductComponent, {
      data: product,
    });

    dialogRef.afterClosed().subscribe(async(productToBeDeleted) => {
      if (productToBeDeleted) {
          const actualProduct = productToBeDeleted();
          await this.cacheService.deleteProduct(actualProduct);
          this.editService.unloadProduct();
      }
    });
  }

  async onImport() {
    if (this.importer()) {
      this.router.navigate([`/import/${this.importer()}`], {
        relativeTo: this.route,
      });
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
      },
    });
  }

  onSave() {
    this.editService.onSave();
    this.searchbar.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      },
    });
  }

  onSelectAndEdit(id: number) {
    const product = this.products().find((p) => p.id == id);
    this.searchbar.clearSuggestions();
    this.searchbar.unsubscribe();
    if (id == 0 || product === undefined) {
      return;
    }
    if (this.selected()?.id === id) {
      this.editService.onEdit();
    } else {
      this.router
        .navigate([`/details/${product.id}`])
        .then(() => this.editService.loadProduct())
        .then(() => {
          this.editService.onEdit();
        });
    }
  }

  onDuplicate(id: number | undefined) {
    if (!id) return;

    const product = this.products().find((p) => p.id == id);
    if (!product) return;

    const dialogRef = this.dialog.open(DialogDuplicateProductComponent, {
      data: product,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.editService.onDuplicate(result as DuplicateProductInfos);
    });
  }

  onRevisions(id: number | undefined) {
    if (!id) return;

    const product = this.products().find((p) => p.id == id);
    if (!product) return;

    const dialogRef = this.dialog.open(DialogShowRevisionsComponent, {
      data: product,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.dialog.open(DialogCreateRevisionComponent, {
          data: product,
        });
      }
    });
  }

  resetFilter(drawer: MatDrawer) {
    this.cacheService.resetfilter();
    drawer.toggle();
  }

  filter(drawer: MatDrawer) {
    this.cacheService.loadProductsForTree();
    drawer.toggle();
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
