/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, signal, untracked, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { ResourceTree } from './components/resource-tree/resource-tree';
import { Router, RouterOutlet } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { environment } from '../environments/environment';
import { ResourceModel, ResourceReferenceModel } from './api/models';
import { ResourceModificationService } from './api/services';
import { DialogAddResource } from './dialogs/dialog-add-resource/dialog-add-resource';
import { ResourceConstructionParameters } from './models/ResourceConstructionParameters';
import './extensions/array.extensions';
import { TranslationConstants } from './translation-constants';
import { CacheResourceService } from './services/cache-resource.service';
import { EditResourceService } from './services/edit-resource.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
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
    ResourceTree,
    RouterOutlet,
    TranslatePipe,
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  host: {
    '(window:beforeunload)': 'beforeUnloadHandler()'
  }
})
export class App {
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private cacheResourceService = inject(CacheResourceService);
  private editResourceService = inject(EditResourceService);
  private modificationService = inject(ResourceModificationService);
  private snackbarService = inject(SnackbarService);

  private readonly trigger = viewChild.required(MatMenuTrigger);
  protected isEditMode = this.editResourceService.editing;
  protected menuTopLeftPosition = signal<Position>({x: '0px', y: '0px'});

  protected readonly resourceToolbarImage = environment.assets + 'assets/resource-toolbar.jpg';

  protected resources = computed(() => this.cacheResourceService.resources() ?? []);
  protected selected = signal<ResourceModel | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;

  protected beforeUnloadHandler() {
    if (this.isEditMode()) {
      this.editResourceService.stashResource();
    }
  }

  constructor() {
    effect(() => {
      const resource = this.editResourceService.activeResource();
      if (this.selected()?.id === resource?.id) {
        return;
      }
      untracked(() => {
        this.select(resource);
      });
    });
  }

  private select(resource: ResourceModel | undefined): void {
    this.selected.set(resource);
  }

  private openContextMenu(resourceId: number, xCoordinate: number, yCoordinate: number) {
    this.trigger().menuData = {id: resourceId};
    this.menuTopLeftPosition.update(() => {
      return {x: `${xCoordinate}px`, y: `${yCoordinate}px`};
    });
    this.trigger().openMenu();
  }

  protected selectResource(id: number) {
    if (this.isEditMode() || this.selected()?.id === id) {
      return;
    }
    this.router.navigate(['details', id]);
  }

  protected clickContainer(event: MouseEvent) {
    if ((event.target as HTMLElement).tagName === 'MAT-TREE') {
      this.onDeselect();
    }
  }

  protected openContextMenuByClicking(event: MouseEvent, resourceId: number) {
    // Only handle right-click, not touch long-press
    if ((event as PointerEvent).pointerType === 'touch') {
      return;
    }
    event.preventDefault();
    this.openContextMenu(resourceId, event.clientX, event.clientY);
  }

  protected onAdd() {
    const parent = this.selected();
    const dialogRef = this.dialog.open(DialogAddResource, {
      height: '560px',
      width: '560px'
    });

    dialogRef.afterClosed().subscribe(async (result: ResourceConstructionParameters | undefined) => {
      if (!result) {
        return;
      }
      const constructed = await this.modificationService
        .constructWithParameters({
          type: result.name,
          method: result.method?.name,
          body: result.method?.parameters,
        })
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));

      if (!constructed) {
        return;
      }
      this.editResourceService.registerNewResource(constructed);
      this.router.navigate(['details', constructed.id]);

      if (!parent) {
        return;
      }

      const referenceToParent = constructed.references?.find(r => r.name == 'Parent');
      if (referenceToParent) {
        referenceToParent.targets = [parent] as ResourceModel[];
      }
      else {
        constructed.references?.push({
          name: 'Parent',
          targets: [parent] as ResourceModel[],
        } as ResourceReferenceModel);
      }
    });
  }

  protected onDelete(resourceId: number | undefined) {
    if (!resourceId) {
      return;
    }

    const resource = this.cacheResourceService.flatResources()?.find(r => r.id === resourceId);
    if (!resource) {
      return;
    }

    const dialogRef = this.dialog.open(DialogRemoveResource, {
      data: resource
    });

    dialogRef.afterClosed().subscribe(async (resourceToBeDeleted) => {
      if (!resourceToBeDeleted) {
        return;
      }

      const actualResource = resourceToBeDeleted;
      this.modificationService
        .remove$Response({id: actualResource.id})
        .then(async () => this.removeResource(actualResource))
        .catch(error => this.snackbarService.handleError(error));
    });
  }

  private removeResource(deletedResource: ResourceModel) {
    this.cacheResourceService.removeResource(deletedResource);
    if (this.selected()?.id === deletedResource.id) {
      this.router.navigate(['']);
    }
  }

  protected onEdit() {
    this.editResourceService.onEdit();
  }

  protected onSelectAndEdit(resourceId: number) {
    this.selectResource(resourceId);
    this.onEdit();
  }

  protected onCancelEditing() {
    if(this.editResourceService.editingUnsavedResource) {
      this.router.navigate(['']);
    }
    else {
      this.editResourceService.onCancel();
    }
  }

  protected onDeselect() {
    this.router.navigate(['']);
  }

  protected async onReload() {
    await this.cacheResourceService.loadResources();
  }

  protected async onSave() {
    await this.editResourceService.onSave();
  }
}

export interface Position {
  x: string;
  y: string;
}
