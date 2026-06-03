/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, computed, input, viewChild, OnInit, linkedSignal } from '@angular/core';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
import { CellState } from 'src/app/api/models/cell-state';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { toSignal } from '@angular/core/rxjs-interop';
import { VisualizableItemModel } from 'src/app/api/models';

@Component({
  selector: 'app-factory',
  templateUrl: './factory.html',
  imports: [
    CommonModule,
    DragDropModule,
    MatIconModule
  ],
  styleUrls: ['./factory.scss']
})
export class Factory implements OnInit {
  private cellStoreService = inject(CellStoreService);
  private factorySelectionService = inject(FactorySelectionService);
  private router = inject(Router);

  private factoryElement = viewChild.required<ElementRef<HTMLElement>>('FactoryElement');
  container = input.required<ElementRef<HTMLElement>>();
  parameters = input.required<VisualizableItemModel>();
  cells = linkedSignal(() => this.cellStoreService.getCells(this.parameters()));
  private editMenuState = toSignal(inject(EditMenuService).activeState$);

  backgroundColor = 'white';

  isHighlighted = computed(() => this.cells().some(x => x.state === CellState.Running));

  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);

  firstWorkingCell = computed(() => this.cells().find(c => c.state === CellState.Running));

  borderColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) return workingCell.orderColor;
    return this.backgroundColor;
  });

  iconColor = computed(() => {
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

  onCellClicked() {
    if (this.editMenuState() !== EditMenuState.Closed) return;

    // ToDo: Move to a RouteResolver to cleanly load and unload data
    this.router.navigate(['/factory', this.parameters().id]).then(() => {
      //close the delails on the right if it is openned
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.parameters().id ?? 0);
    });
  }

  onCellMove(event: CdkDragEnd<any>) {
    const params = this.parameters();
    const factoryElement = this.factoryElement();
    const containerElement = this.container();

    if (!params.location) return;

    // Calculate new position as percetage value relative to the cell-container
    const cellY = factoryElement.nativeElement.offsetTop + event.distance.y;
    const cellX = factoryElement.nativeElement.offsetLeft + event.distance.x;
    const containerHeight = containerElement.nativeElement.offsetHeight;
    const containerWidth = containerElement.nativeElement.offsetWidth;

    const updatedLocation = {
      ...this.parameters().location,
      positionX: this.clamp(cellX / containerWidth),
      positionY: this.clamp(cellY / containerHeight)
    };

    // Save position and reset translation
    this.cellStoreService.moveCell(updatedLocation);
    event.source._dragRef.reset();
  }

  private clamp(x: number) {
    return Math.max(0, Math.min(x, 1));
  }
}
