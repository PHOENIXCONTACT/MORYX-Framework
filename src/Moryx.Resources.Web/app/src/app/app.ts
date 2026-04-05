/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, effect, inject, OnDestroy, OnInit, signal, untracked, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { MatTreeFlatDataSource, MatTreeFlattener, MatTreeModule } from '@angular/material/tree';
import { Router, RouterOutlet } from '@angular/router';
import {
  LanguageService,
  SnackbarService} from '@moryx/ngx-web-framework/services';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { ResourceModel, ResourceReferenceModel } from './api/models';
import { ResourceModificationService } from './api/services';
import { DialogAddResource } from './dialogs/dialog-add-resource/dialog-add-resource';
import { ResourceConstructionParameters } from './models/ResourceConstructionParameters';
import './extensions/array.extensions';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { CacheResourceService } from './services/cache-resource.service';
import { EditResourceService } from './services/edit-resource.service';
import { FormControlService } from './services/form-control-service.service';
import { FlatNode, SessionService } from './services/session.service';
import { lastValueFrom, Subscription } from 'rxjs';
import { getHierarchieLineFor } from './models/TypeTree';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DialogRemoveResource } from "./dialogs/dialog-remove-resource/dialog-remove-resource";
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
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
  host: {
    '(window:beforeunload)': 'beforeUnloadHander()'
  }
})
export class App implements OnInit, OnDestroy {
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private cacheResourceService = inject(CacheResourceService);
  private editResourceService = inject(EditResourceService);
  private modificationService = inject(ResourceModificationService);
  private sessionService = inject(SessionService);
  private translateService = inject(TranslateService);
  private languageService = inject(LanguageService);
  private snackbarService = inject(SnackbarService);
  private formControlService = inject(FormControlService);

  private readonly trigger = viewChild.required(MatMenuTrigger);
  isEditMode = toSignal(this.editResourceService.edit$, { initialValue: false });
  menuTopLeftPosition = signal<Position>({x: '0px', y: '0px'});

  readonly resourceToolbarImage = environment.assets + 'assets/resource-toolbar.jpg';

  resources?: ResourceModel[];
  resourcesFlat?: ResourceModel[];
  selected = signal<ResourceModel | undefined>(undefined);
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

  beforeUnloadHander() {
    if (this.isEditMode()) this.editResourceService.stashResource();
    this.sessionService.storeTreeState(this.treeControl);
  }

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translateService.setFallbackLang('en');
    this.translateService.use(this.languageService.getDefaultLanguage());
    this.formControlService.canSave.subscribe(state => (this.canSave = state));
    
    effect(() => {
      const resource = this.editResourceService.activeResource();
      if (this.selected()?.id === resource?.id) {
        return;
      }
      untracked(() => this.select(resource));
    });
  }

  ngOnDestroy(): void {
    this.formControlService.canSave.unsubscribe();
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  async ngOnInit() {
    this.subscriptions.push(
      this.cacheResourceService.resources.subscribe(resources => {
        if (this.treeStateIsInitialized) this.sessionService.storeTreeState(this.treeControl);
        else this.treeStateIsInitialized = true;
        this.resources = resources;
        this.dataSource.data = resources ?? [];
        this.sessionService.restoreTreeState(this.treeControl);
      })
    );

    this.subscriptions.push(
      this.cacheResourceService.flatResources.subscribe(resources => {
        this.resourcesFlat = resources;
      })
    );
  }

  private select(resource: ResourceModel | undefined): void {
    this.selected.set(resource);
    if (this.treeStateIsInitialized || !resource) return;
    this.expandSelectedBranch();
  }

  private expandSelectedBranch() {
    const toExpand = getHierarchieLineFor(this.selected()?.id, this.resources);
    this.treeControl.dataNodes.filter(n => toExpand.find(e => e === n.id)).forEach(n => this.treeControl.expand(n));
    this.treeStateIsInitialized = true;
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
    if (this.isEditMode() || this.selected()?.id === id) return;
    this.router.navigate(['details', id]);
  }

  clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName === 'MAT-TREE') this.onDeselect();
  }

  openContextMenuByClicking(event: MouseEvent, resourceId: number) {
    event.preventDefault();
    this.openContextMenu(resourceId, event.clientX, event.clientY);
  }

  onAdd() {
    const parent = this.selected();
    const dialogRef = this.dialog.open(DialogAddResource, {
      height: '560px',
      width: '560px'
    });

    dialogRef.afterClosed().subscribe(async (result: ResourceConstructionParameters | undefined) => {
      if (!result) return;
      const constructed = await lastValueFrom(this.modificationService
        .constructWithParameters({
          type: result.name,
          method: result.method?.name,
          body: result.method?.parameters,
        }))
      .catch(async (e: HttpErrorResponse) => await this.snackbarService.handleError(e));
      
      if (!constructed) return;
      this.editResourceService.registerNewResource(constructed);
      this.router.navigate(['details', constructed.id]);

      if (!parent) return;

      const referenceToParent = constructed.references?.find(r => r.name == 'Parent');
      if (referenceToParent) referenceToParent.targets = [parent] as ResourceModel[];
      else
        constructed.references?.push({
          name: 'Parent',
          targets: [parent] as ResourceModel[],
        } as ResourceReferenceModel);
    });
  }

  onDelete(resourceId: number | undefined) {
    if (!resourceId) return;

    const resource = this.resourcesFlat?.find(r => r.id === resourceId);
    if (!resource) return;

    const dialogRef = this.dialog.open(DialogRemoveResource, {
      data: resource
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
    this.cacheResourceService.removeResource(deletedResource);
    if (this.selected()?.id === deletedResource.id)
      this.router.navigate(['']);
  }

  onEdit() {
    this.editResourceService.onEdit();
  }

  onSelectAndEdit(resourceId: number) {
    this.selectResource(resourceId);
    this.onEdit();
  }

  onCancelEditing() {
    if(this.editResourceService.editingUnsavedResource)
      this.router.navigate(['']);
    else
      this.editResourceService.onCancel();
  }

  onDeselect() {
    this.router.navigate(['']);
  }

  async onReload() {
    await this.cacheResourceService.loadResources();
  }

  async onSave() {
    await this.editResourceService.onSave();
  }
}

export interface Position {
  x: string;
  y: string;
}
