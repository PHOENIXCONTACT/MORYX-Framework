/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, signal, computed, input, viewChild } from '@angular/core';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { FactoryStateModel } from 'src/app/api/models/factory-state-model';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
import { CellState } from 'src/app/api/models/cell-state';
import CellModel from 'src/app/models/cellModel';
import { VisualizableItemModel } from 'src/app/api/models/visualizable-item-model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

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
export class Factory {
  private cellStoreService = inject(CellStoreService);
  private editMenuService = inject(EditMenuService);
  private factorySelectionService = inject(FactorySelectionService);
  private router = inject(Router);

  cellElement = viewChild<ElementRef<HTMLElement>>('FactoryElement');
  container = input.required<ElementRef<HTMLElement>>();
  parameters = input.required<VisualizableItemModel>();
  factory = computed(() => this.parameters() as FactoryStateModel);
  cells = signal<CellModel[]>([]);
  private editMenuState = signal<EditMenuState>(EditMenuState.Closed);

  backgroundColor = 'white';

  isHighlighted = computed(() => this.cells().some(x => x.state === CellState.Running));

  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);

  firstWorkingCell = computed(() => this.cells().find(c => c.state === CellState.Running));

  borderColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) return workingCell.orderColor;
    return 'white';
  });

  iconColor = computed(() => {
    const workingCell = this.firstWorkingCell();
    if (this.isHighlighted() && workingCell?.orderColor) return workingCell.orderColor;
    return '#585858';
  });

  ngOnInit(): void {
    // Keep the menu state
    this.editMenuService.activeState$.subscribe({
      next: state => this.editMenuState.set(state)
    });
    this.cellStoreService.cells$.subscribe(c => this.cells.set(c.filter(cell => cell.factoryId === this.factory().id)));
    // React to updates to the cell data
    this.cellStoreService.cellUpdated$.subscribe(cell => {
      if (!cell) return;
      if (cell.id != this.factory().id && !this.cells().some(c => c.id === cell.id)) return;

      this.updateFactoryCell(cell);
    });
  }

  updateFactoryCell(cell: CellModel) {
    this.cells.update(cells => {
      const cellToUpdate = cells.find(c => c.id === cell.id);
      if (!cellToUpdate) return cells;

      cellToUpdate.iconName = cell.iconName;
      cellToUpdate.image = cell.image;
      cellToUpdate.name = cell.name;
      cellToUpdate.propertySettings = cell.propertySettings;
      cellToUpdate.state = cell.state;
      cellToUpdate.classification = cell.classification;
      cellToUpdate.orderNumber = cell.orderNumber;
      cellToUpdate.operationNumber = cell.operationNumber;
      cellToUpdate.orderColor = cell.orderColor;
      return [...cells];
    });
  }

  onCellClicked() {
    if (this.editMenuState() !== EditMenuState.Closed) return;

    this.router.navigate(['/factory', this.factory().id]).then(() => {
      //close the delails on the right if it is openned
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.factory().id ?? 0);
    });
  }

  onCellMove(event: CdkDragEnd<any>) {
    if (!this.factory().location) return;

    // Calculate new position as percetage value relative to the cell-container
    const cellY = this.cellElement()!.nativeElement.offsetTop + event.distance.y;
    const cellX = this.cellElement()!.nativeElement.offsetLeft + event.distance.x;
    const containerHeight = this.container().nativeElement.offsetHeight;
    const containerWidth = this.container().nativeElement.offsetWidth;

    const updatedLocation = {
      ...this.factory().location,
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

