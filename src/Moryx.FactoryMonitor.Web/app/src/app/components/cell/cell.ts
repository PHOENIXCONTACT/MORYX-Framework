/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, ElementRef, viewChild, input, computed, signal, effect, untracked, inject } from '@angular/core';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CellState } from '../../api/models/cell-state';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import CellModel from 'src/app/models/cellModel';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-cell',
  templateUrl: './cell.html',
  styleUrls: ['./cell.scss'],
  imports: [
    CommonModule,
    MatIcon,
    DragDropModule
  ]
})
export class Cell implements OnInit {
  private cellStoreService = inject(CellStoreService);
  private orderStoreService = inject(OrderStoreService);
  private editMenuService = inject(EditMenuService);

  cellElement = viewChild<ElementRef<HTMLElement>>('cell');
  container = input<ElementRef<HTMLElement>>();
  parameters = input.required<CellModel>();
  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);
  private editMenuState = signal<EditMenuState | undefined>(undefined);
  currentCell = signal<CellModel | undefined>(undefined);
  isHighlighted = signal<boolean>(true);
  backgroundColor = computed(() =>
    this.currentCell()?.state === CellState.NotReadyToWork ? '#e46d6d' : 'white'
  );
  borderColor = computed(() => {
    if (this.isHighlighted() && this.currentCell()!.orderColor)
      return this.currentCell()?.orderColor!;
    if (this.currentCell()?.state === CellState.NotReadyToWork)
      return '#e46d6d';
    return 'white';
  });
  iconColor = computed(() => {
    if (this.isHighlighted() && this.currentCell()?.orderColor)
      return this.currentCell()?.orderColor!;
    if (this.currentCell()?.state === CellState.NotReadyToWork)
      return 'white';
    return '#585858';
  });

  constructor() {
    effect(() => {
      const parameters = this.parameters();
      untracked(() => {
        this.updateCell(parameters);
      });
    });
  }

  ngOnInit(): void {
    // React to toggling of an order
    this.orderStoreService.toggledOrder$.subscribe(o => {
      if (this.currentCell()?.orderNumber !== o.orderNumber || this.currentCell()?.operationNumber !== o.operationNumber)
        return;
      this.isHighlighted.set(o.isToggled);
    });

    // Keep the menu state
    this.editMenuService.activeState$.subscribe({
      next: state => (this.editMenuState.set(state))
    });

    // React to updates to the cell data
    this.cellStoreService.cellUpdated$.subscribe(c => {
      if (c?.id != this.currentCell()?.id) return;
      this.updateCell(c!);
    });
  }

  private updateCell(newParams: CellModel) {
    this.currentCell.set(newParams);
    if (this.currentCell()?.orderNumber && this.currentCell()?.operationNumber)
      if (newParams.state == CellState.Running &&
        this.orderStoreService.getOrder(this.currentCell()?.orderNumber!, this.currentCell()?.operationNumber!)?.isToggled) {
        this.isHighlighted.set(true);
      } else {
        this.isHighlighted.set(false);
      }
  }

  onCellClicked() {
    //Do not show details menu if the edit button is not closed
    if (this.editMenuState() != EditMenuState.Closed) return;
    this.cellStoreService.selectCell(this.currentCell()?.id!);
  }

  onCellMove(event: CdkDragEnd<any>) {
    // Calculate new position as percetage value relative to the cell-container
    const cellY = this.cellElement()?.nativeElement?.offsetTop! + event.distance.y;
    const cellX = this.cellElement()?.nativeElement?.offsetLeft! + event.distance.x;
    const containerHeight = this.container()?.nativeElement?.offsetHeight!;
    const containerWidth = this.container()?.nativeElement?.offsetWidth!;
    this.currentCell.update(cell => {
      cell!.location!.positionX = this.clamp(cellX / containerWidth);
      cell!.location!.positionY = this.clamp(cellY / containerHeight);
      return cell;
    });

    // Save position and reset translation
    this.cellStoreService.moveCell(this.currentCell()?.location!);
    event.source._dragRef.reset();
  }

  private clamp(x: number) {
    return Math.max(0, Math.min(x, 1));
  }
}

