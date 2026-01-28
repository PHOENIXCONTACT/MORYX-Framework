/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {FlatTreeControl} from '@angular/cdk/tree';
import {Component, HostListener, OnDestroy, OnInit, signal, viewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatMenuModule, MatMenuTrigger} from '@angular/material/menu';
import {MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule} from '@angular/material/tree';
import {Router, RouterOutlet} from '@angular/router';
import {
  LanguageService,
  SnackbarService,
  SearchBarService,
  SearchRequest,
  SearchSuggestion
} from '@moryx/ngx-web-framework/services';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {environment} from 'src/environments/environment';
import {ResourceModel, ResourceTypeModel} from './api/models';
import {ResourceModificationService} from './api/services';
import {DialogAddResourceComponent} from './dialogs/dialog-add-resource/dialog-add-resource.component';
import {ResourceConstructionParameters} from './models/ResourceConstructionParameters';
import './extensions/array.extensions';
import {TranslationConstants} from './extensions/translation-constants.extensions';
import {CacheResourceService} from './services/cache-resource.service';
import {EditResourceService} from './services/edit-resource.service';
import {FormControlService} from './services/form-control-service.service';
import {FlatNode, SessionService} from './services/session.service';
import {Subscription} from 'rxjs';
import {getHierarchieLineFor} from './models/TypeTree';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatIconModule} from '@angular/material/icon';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatTooltipModule} from '@angular/material/tooltip';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {DialogRemoveResourceComponent} from "./dialogs/dialog-remove-resource/dialog-remove-resource.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatSelectModule,
    MatSidenavModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTreeModule,
    RouterOutlet,
    TranslateModule,
  ],
  standalone: true,
})
export class AppComponent implements OnInit, OnDestroy {
  private readonly trigger = viewChild.required(MatMenuTrigger);
  menuTopLeftPosition = signal<Position>({x: '0px', y: '0px'});

  readonly resourceToolbarImage = environment.assets + 'assets/resource-toolbar.jpg';

  resources?: ResourceModel[];
  resourcesFlat?: ResourceModel[];
  selected?: ResourceModel;
  title = 'Moryx.Resources.Web';
  canSave!: boolean;
  TranslationConstants = TranslationConstants;
  private treeStateIsInitialized: boolean = false;
  private subscriptions: Subscription[] = [];

  private _transformer = (node: ResourceModel, level: number) => {
    const childReferences = node.references?.find(ref => ref.name == 'Children')?.targets ?? [];
    return {
      expandable: !!childReferences.length,
      name: node.name,
      level: level,
      id: node.id,
    } as FlatNode;
  };

  treeControl = new FlatTreeControl<FlatNode>(
    node => node.level,
    node => node.expandable
  );

  treeFlattener = new MatTreeFlattener(
    this._transformer,
    node => node.level,
    node => node.expandable,
    node => {
      if (node.references) return node.references.find(ref => ref.name == 'Children')?.targets;
      else return null;
    }
  );

  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  hasChild = (_: number, node: FlatNode) => node.expandable;

  @HostListener('window:beforeunload')
  beforeUnloadHander() {
    if (this.editService.edit) this.editService.stashResource();
    this.sessionService.storeTreeState(this.treeControl);
  }

  constructor(
    private router: Router,
    public dialog: MatDialog,
    private cacheService: CacheResourceService,
    public editService: EditResourceService,
    private modificationService: ResourceModificationService,
    private sessionService: SessionService,
    public translate: TranslateService,
    private searchBarService: SearchBarService,
    private languageService: LanguageService,
    private snackbarService: SnackbarService,
    private formControlService: FormControlService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translate.setFallbackLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
    this.formControlService.canSave.subscribe(state => (this.canSave = state));
  }

  ngOnDestroy(): void {
    this.searchBarService.unsubscribe();
    this.formControlService.canSave.unsubscribe();
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  async ngOnInit() {
    this.subscriptions.push(
      this.cacheService.resources.subscribe(resources => {
        if (this.treeStateIsInitialized) this.sessionService.storeTreeState(this.treeControl);
        else this.treeStateIsInitialized = true;
        this.resources = resources;
        this.dataSource.data = resources ?? [];
        this.sessionService.restoreTreeState(this.treeControl);
      })
    );

    this.subscriptions.push(
      this.cacheService.flatResources.subscribe(resources => {
        this.resourcesFlat = resources;
      })
    );

    this.subscriptions.push(this.editService.activeResource$.subscribe(resource => this.select(resource)));

    // ToDo: move to edit service
    const wipResource = this.sessionService.getWipResource();
    if (wipResource) {
      this.editService.loadFromStorage();
    } else {
      this.editService.loadResource();
    }

    this.searchBarService.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      },
    });
  }

  private select(resource: ResourceModel | undefined): void {
    this.selected = resource;
    if (this.treeStateIsInitialized || !resource) return;
    this.expandSelectedBranch();
  }

  private expandSelectedBranch() {
    const toExpand = getHierarchieLineFor(this.selected?.id, this.resources);
    this.treeControl.dataNodes.filter(n => toExpand.find(e => e === n.id)).forEach(n => this.treeControl.expand(n));
    this.treeStateIsInitialized = true;
  }

  onSearch(result: SearchRequest) {
    const urlBase = 'Resources/details/';
    if (!this.resourcesFlat) return;

    const searchTerm = result.term;
    let resources = this.resourcesFlat.filter(r => r.name?.toLowerCase()?.includes(searchTerm.toLowerCase()));
    if (!resources) resources = [];

    if (result.submitted) {
      this.searchBarService.clearSuggestions();
      if (resources.length === 1 && resources[0].id) this.selectResource(resources[0].id);
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (let resource of resources) {
        if (!resource.name) continue;

        const url = urlBase + resource.id;
        searchSuggestions.push({text: resource.name, url: url});
      }

      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  openContextMenuByPressing(event: any, id: number) {
    this.openContextMenu(id, event.pointers[0].clientX, event.pointers[0].clientY);
  }

  private openContextMenu(resourceId: number, xCoordinate: number, yCoordinate: number) {
    this.trigger().menuData = {id: resourceId};
    this.menuTopLeftPosition.update(() => {
      return {x: `${xCoordinate}px`, y: `${yCoordinate}px`};
    });
    this.trigger().openMenu();
  }

  selectResource(id: number) {
    if (this.editService.edit || this.selected?.id === id) return;
    this.router.navigate([`/details/${id}`]).then(() => this.editService.loadResource());
  }

  clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName === 'MAT-TREE') this.onDeselect();
  }

  openContextMenuByClicking(event: MouseEvent, resourceId: number) {
    event.preventDefault();
    this.openContextMenu(resourceId, event.clientX, event.clientY);
  }

  onAdd() {
    const dialogRef = this.dialog.open(DialogAddResourceComponent, {
      height: '560px',
      width: '560px',
    });

    dialogRef.afterClosed().subscribe(async (result: ResourceConstructionParameters | undefined) => {
      if (!result) return;
      var model = await this.editService.addNewResource(result, this.selected);
      if (model)
        this.navigateToResource(model);
    });
  }

  private navigateToResource(resourceModel: ResourceModel) {
    if (resourceModel.properties) this.router.navigate([`/details/${resourceModel.id}/properties`]);
    else this.router.navigate([`/details/${resourceModel.id}`]);
  }

  onDelete(resourceId: number | undefined) {
    if (!resourceId) return;

    const resource = this.resourcesFlat?.find(r => r.id === resourceId);
    if (!resource) return;

    const dialogRef = this.dialog.open(DialogRemoveResourceComponent, {
      data: resource,
    });

    dialogRef.afterClosed().subscribe(async (resourceToBeDeleted) => {
      if (!resourceToBeDeleted) return;

      const actualResource = resourceToBeDeleted;
      this.modificationService
        .remove$Response({id: actualResource.id})
        .toAsync()
        .then(async () => this.removeResource(actualResource))
        .catch(async error => await this.snackbarService.handleError(error));
    });
  }

  private removeResource(deletedResource: ResourceModel) {
    this.cacheService.removeResource(deletedResource);
    if (this.selected?.id === deletedResource.id) this.editService.removeResource();
  }

  onEdit() {
    this.searchBarService.clearSuggestions();
    this.searchBarService.unsubscribe();
    this.editService.onEdit();
  }

  onSelectAndEdit(resourceId: number) {
    this.selectResource(resourceId);
    this.onEdit();
  }

  onCancelEditing() {
    this.editService.onCancel();
    this.searchBarService.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      },
    });
  }

  onDeselect() {
    if (this.editService.edit) {
      this.searchBarService.subscribe({
        next: (result: SearchRequest) => {
          this.onSearch(result);
        },
      });
    }
    this.editService.onDeselect();
  }

  async onReload() {
    await this.cacheService.loadResources();
  }

  async onSave() {
    await this.editService.onSave();
    this.searchBarService.subscribe({
      next: (result: SearchRequest) => {
        this.onSearch(result);
      },
    });
  }
}


export interface Position {
  x: string;
  y: string;
}

