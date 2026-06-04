/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, OnDestroy, ElementRef, viewChild, input, computed, inject, linkedSignal } from '@angular/core';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CellState } from '../../api/models/cell-state';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import CellModel from 'src/app/models/cellModel';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { VisualizableItemModel } from 'src/app/api/models';
import { toSignal } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';
import { createUpdatedLocation } from 'src/app/extensions/locations';

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
export class Cell implements OnInit, OnDestroy {
  private cellStoreService = inject(CellStoreService);
  private orderStoreService = inject(OrderStoreService);
  private editMenuService = inject(EditMenuService);

  private subscriptions = new Subscription();

  cellElement = viewChild.required<ElementRef<HTMLElement>>('cell');
  container = input.required<ElementRef<HTMLElement>>();
  parameters = input.required<VisualizableItemModel>();
  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);
  private editMenuState = toSignal(this.editMenuService.activeState$);
  currentCell = linkedSignal<CellModel>(() => this.cellStoreService.getCell(this.parameters().id!));
  private currentOrder = computed(() => {
    const cell = this.currentCell();
    if (!cell?.orderNumber || !cell.operationNumber) return null;
    return this.orderStoreService.getOrder(cell.orderNumber, cell.operationNumber);
  });
  private currentOrderIsToggled = linkedSignal(() => !!this.currentOrder()?.isToggled);
  isHighlighted = computed(() => {
    const cell = this.currentCell();
    return !!cell && cell.state == CellState.Running && !!cell.orderNumber && !!cell.operationNumber &&
        this.currentOrderIsToggled();
  });
  backgroundColor = computed(() =>
    this.currentCell()?.state === CellState.NotReadyToWork ? '#e46d6d' : 'white'
  );
  borderColor = computed(() => {
    const cell = this.currentCell();
    if (this.isHighlighted() && cell.orderColor)
      return cell.orderColor!;
    if (cell.state === CellState.NotReadyToWork)
      return '#e46d6d';
    return 'white';
  });
  iconColor = computed(() => {
    const cell = this.currentCell();
    if (this.isHighlighted() && cell.orderColor)
      return cell.orderColor!;
    if (cell.state === CellState.NotReadyToWork)
      return 'white';
    return '#585858';
  });

  ngOnInit(): void {
    // React to toggling of an order
    this.subscriptions.add(this.orderStoreService.toggledOrder$.subscribe(o => {
      if (this.currentOrder()?.orderNumber !== o.orderNumber || this.currentOrder()?.operationNumber !== o.operationNumber)
        return;
      this.currentOrderIsToggled.set(o.isToggled);
    }));

    // React to updates to the cell data
    this.subscriptions.add(this.cellStoreService.cellUpdated$.subscribe(c => {
      if (c.id !== this.currentCell()?.id) {
        return;
      }

      this.currentCell.set({... c});
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  onCellClicked() {
    //Do not show details menu if the edit button is not closed
    if (this.editMenuState() != EditMenuState.Closed) return;
    this.cellStoreService.selectCell(this.currentCell().id!);
  }

  async onCellMove(event: CdkDragEnd<any>) {
    const params = this.parameters();

    // Calculate new position as percetage value relative to the cell-container
    const updatedLocation = createUpdatedLocation(event, this.cellElement(), 
      this.container(), params.location?.id);

    // Save position and reset translation as the new position is immediately applied
    await this.cellStoreService.moveItem(params, updatedLocation);
    event.source._dragRef.reset();
  }  
}
