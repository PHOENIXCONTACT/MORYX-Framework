/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { Component, computed, ElementRef, inject, input, linkedSignal, OnDestroy, OnInit, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatIcon } from '@angular/material/icon';
import { Subscription } from 'rxjs';
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
export class Cell implements OnInit, OnDestroy {
  private cellStoreService = inject(CellStoreService);
  private orderStoreService = inject(OrderStoreService);
  private editMenuService = inject(EditMenuService);

  private subscriptions = new Subscription();

  readonly cellElement = viewChild.required<ElementRef<HTMLElement>>('cell');
  readonly container = input.required<ElementRef<HTMLElement>>();
  readonly parameters = input.required<VisualizableItemModel>();
  protected isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);
  private editMenuState = toSignal(this.editMenuService.activeState$);
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

  ngOnInit(): void {
    // React to toggling of an order
    this.subscriptions.add(this.orderStoreService.toggledOrder$.subscribe(o => {
      if (this.currentOrder()?.orderNumber !== o.orderNumber || this.currentOrder()?.operationNumber !== o.operationNumber) {
        return;
      }
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

  protected onCellClicked() {
    //Do not show details menu if the edit button is not closed
    if (this.editMenuState() != EditMenuState.Closed) {
      return;
    }
    this.cellStoreService.selectCell(this.currentCell().id!);
  }

  protected async onCellMove(event: CdkDragEnd<any>) {
    const params = this.parameters();

    // Calculate new position as percetage value relative to the cell-container
    const updatedLocation = createUpdatedLocation(event, this.cellElement(),
      this.container(), params.location?.id);

    // Save position and reset translation as the new position is immediately applied
    await this.cellStoreService.moveItem(params, updatedLocation);
    event.source._dragRef.reset();
  }
}
