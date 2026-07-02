/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, computed, input, viewChild, OnInit, linkedSignal, ChangeDetectionStrategy } from '@angular/core';
import { CellStoreService } from '@app/services/cell-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { EditMenuState } from '@app/services/EditMenutState';
import { EditMenuService } from '@app/services/edit-menu.service';
import { FactorySelectionService } from '@app/services/factory-selection.service';
import { CellState } from '@api/models/cell-state';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { toSignal } from '@angular/core/rxjs-interop';
import { VisualizableItemModel } from '@api/models';
import { createUpdatedLocation } from '@app/extensions/locations';
import { lastValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { FactoryMonitorService } from '@api/services';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

@Component({
  selector: 'app-factory',
  templateUrl: './factory.html',
  imports: [
    CommonModule,
    DragDropModule,
    MatIconModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./factory.scss']
})
export class Factory implements OnInit {
  private cellStoreService = inject(CellStoreService);
  private factorySelectionService = inject(FactorySelectionService);
  private factoryMonitorService = inject(FactoryMonitorService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);

  private factoryElement = viewChild.required<ElementRef<HTMLElement>>('FactoryElement');
  readonly container = input.required<ElementRef<HTMLElement>>();
  readonly parameters = input.required<VisualizableItemModel>();
  protected cells = linkedSignal(() => this.cellStoreService.getCells(this.parameters()));
  private editMenuState = toSignal(inject(EditMenuService).activeState$);

  protected backgroundColor = 'white';

  protected isHighlighted = computed(() => this.cells().some(x => x.state === CellState.Running));

  protected isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);

  protected firstWorkingCell = computed(() => this.cells().find(c => c.state === CellState.Running));

  protected borderColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) return workingCell.orderColor;
    return this.backgroundColor;
  });

  protected iconColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) return workingCell.orderColor;
    return '#585858';
  });

  ngOnInit(): void {
    // React to updates to the cell data
    this.cellStoreService.cellUpdated$.subscribe(cell => {
      if (cell.factoryId != this.parameters().id) {
        return;
      }

      this.cells.update(cells => {
        const index = cells.findIndex(c => c.id === cell.id);
        cells[index] = cell;
        return [... cells];
      });
    });
  }

  protected onCellClicked() {
    if (this.editMenuState() !== EditMenuState.Closed) return;

    // ToDo: Move to a RouteResolver to cleanly load and unload data
    this.router.navigate(['/factory', this.parameters().id]).then(() => {
      //close the delails on the right if it is openned
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.parameters().id ?? 0);
    });
  }

  protected async onCellMove(event: CdkDragEnd<any>) {
    const params = this.parameters();

    // Calculate new position as percetage value relative to the cell-container
    const updatedLocation = createUpdatedLocation(event, this.factoryElement(),
      this.container(), params.location?.id);

    // Save position and reset translation
    try {
      await lastValueFrom(this.factoryMonitorService.moveCell({ body: updatedLocation }));
    } catch (error) {
      this.snackbarService.handleError(error as HttpErrorResponse);
      return;
    }
  }
}
