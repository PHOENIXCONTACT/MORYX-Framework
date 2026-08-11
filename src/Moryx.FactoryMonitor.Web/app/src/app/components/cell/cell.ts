/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { Component, computed, effect, ElementRef, inject, input, linkedSignal, untracked, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { VisualizableItemModel } from '@api/models';
import { createUpdatedLocation } from '@app/extensions/locations';
import CellModel from '@app/models/cellModel';
import { EditMenuState } from '@app/services/EditMenutState';
import { CellStoreService } from '@app/services/cell-store.service';
import { EditMenuService } from '@app/services/edit-menu.service';
import { OrderStoreService } from '@app/services/order-store.service';
import { CellState } from '@api/models/cell-state';

@Component({
  selector: 'app-cell',
  templateUrl: './cell.html',
  styleUrls: ['./cell.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatIcon,
    DragDropModule
  ]
})
export class Cell {
  private cellStoreService = inject(CellStoreService);
  private orderStoreService = inject(OrderStoreService);
  private editMenuService = inject(EditMenuService);

  constructor() {
    // React to updates to the cell data
    effect(() => {
      const c = this.cellStoreService.cellUpdated();
      untracked(() => {
        if (!c || c.id !== this.currentCell()?.id) {
          return;
        }
        this.currentCell.set({... c});
      });
    });

    // React to toggling of an order
    effect(() => {
      const o = this.orderStoreService.toggledOrder();
      if (!o || this.currentOrder()?.orderNumber !== o.orderNumber || this.currentOrder()?.operationNumber !== o.operationNumber) {
        return;
      }
      this.currentOrderIsToggled.set(o.isToggled);
    });
  }

  readonly cellElement = viewChild.required<ElementRef<HTMLElement>>('cell');
  readonly container = input.required<ElementRef<HTMLElement>>();
  readonly parameters = input.required<VisualizableItemModel>();
  protected isEditMode = computed(() => this.editMenuService.activeState() === EditMenuState.EditingCells);
  protected currentCell = linkedSignal<CellModel>(() => this.cellStoreService.getCell(this.parameters().id!));
  private currentOrder = computed(() => this.orderStoreService.getOrder(this.currentCell()));
  private currentOrderIsToggled = linkedSignal(() => !!this.currentOrder()?.isToggled);
  protected isHighlighted = computed(() => {
    const cell = this.currentCell();
    return !!cell && cell.state == CellState.Running && !!cell.orderNumber && !!cell.operationNumber &&
        this.currentOrderIsToggled();
  });
  protected backgroundColor = computed(() =>
    this.currentCell()?.state === CellState.NotReadyToWork ? '#e46d6d' : 'white'
  );
  protected borderColor = computed(() => {
    const cell = this.currentCell();
    if (this.isHighlighted() && cell.orderColor) {
      return cell.orderColor!;
    }
    if (cell.state === CellState.NotReadyToWork) {
      return '#e46d6d';
    }
    return 'white';
  });
  protected iconColor = computed(() => {
    const cell = this.currentCell();
    if (this.isHighlighted() && cell.orderColor) {
      return cell.orderColor!;
    }
    if (cell.state === CellState.NotReadyToWork) {
      return 'white';
    }
    return '#585858';
  });

  protected onCellClicked() {
    //Do not show details menu if the edit button is not closed
    if (this.editMenuService.activeState() != EditMenuState.Closed) {
      return;
    }
    this.cellStoreService.selectCell(this.currentCell().id!);
  }

  protected async onCellMove(event: CdkDragEnd) {
    const params = this.parameters();

    // Calculate new position as percetage value relative to the cell-container
    const updatedLocation = createUpdatedLocation(event, this.cellElement(),
      this.container(), params.location?.id);

    // Save position and reset translation as the new position is immediately applied
    await this.cellStoreService.moveItem(params, updatedLocation);
    event.source._dragRef.reset();
  }
}
