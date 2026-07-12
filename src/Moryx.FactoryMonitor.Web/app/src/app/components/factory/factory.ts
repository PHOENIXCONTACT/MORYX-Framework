/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, computed, effect, input, viewChild, linkedSignal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { CellStoreService } from '@app/services/cell-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { EditMenuState } from '@app/services/EditMenutState';
import { EditMenuService } from '@app/services/edit-menu.service';
import { FactorySelectionService } from '@app/services/factory-selection.service';
import { CellState } from '@api/models/cell-state';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { VisualizableItemModel } from '@api/models';
import { createUpdatedLocation } from '@app/extensions/locations';

import { HttpErrorResponse } from '@angular/common/http';
import { FactoryMonitorService } from '@api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

@Component({
  selector: 'app-factory',
  templateUrl: './factory.html',
  imports: [
    DragDropModule,
    MatIconModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./factory.scss']
})
export class Factory {
  private cellStoreService = inject(CellStoreService);
  private factorySelectionService = inject(FactorySelectionService);
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);
  private editMenuService = inject(EditMenuService);

  private factoryElement = viewChild.required<ElementRef<HTMLElement>>('FactoryElement');
  readonly container = input.required<ElementRef<HTMLElement>>();
  readonly parameters = input.required<VisualizableItemModel>();
  protected cells = linkedSignal(() => this.cellStoreService.getCells(this.parameters()));
  protected backgroundColor = 'white';

  protected isHighlighted = computed(() => this.cells().some(x => x.state === CellState.Running));

  protected isEditMode = computed(() => this.editMenuService.activeState() === EditMenuState.EditingCells);

  protected firstWorkingCell = computed(() => this.cells().find(c => c.state === CellState.Running));

  protected borderColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) {
      return workingCell.orderColor;
    }
    return this.backgroundColor;
  });

  protected iconColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) {
      return workingCell.orderColor;
    }
    return '#585858';
  });

  constructor() {
    // React to updates to the cell data
    effect(() => {
      const cell = this.cellStoreService.cellUpdated();
      untracked(() => {
        if (!cell || cell.factoryId != this.parameters().id) {
          return;
        }

        this.cells.update(cells => {
          const index = cells.findIndex(c => c.id === cell.id);
          cells[index] = cell;
          return [... cells];
        });
      });
    });
  }

  protected onCellClicked() {
    if (this.editMenuService.activeState() !== EditMenuState.Closed) {
      return;
    }

    // ToDo: Move to a RouteResolver to cleanly load and unload data
    this.router.navigate(['/factory', this.parameters().id]).then(() => {
      //close the delails on the right if it is openned
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.parameters().id ?? 0);
    });
  }

  protected async onCellMove(event: CdkDragEnd) {
    const params = this.parameters();

    // Calculate new position as percetage value relative to the cell-container
    const updatedLocation = createUpdatedLocation(event, this.factoryElement(),
      this.container(), params.location?.id);

    // Save position and reset translation
    try {
      await this.factoryMonitorService.moveCell({ body: updatedLocation });
    } catch (error) {
      this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }
  }
}
