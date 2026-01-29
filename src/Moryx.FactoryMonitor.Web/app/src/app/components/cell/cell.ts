/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, ElementRef, viewChild, input, computed, signal, effect, untracked } from '@angular/core';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { CellState } from '../../api/models/cell-state';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { CellSettingsService } from 'src/app/services/cell-settings.service';
import CellModel from 'src/app/models/cellModel';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
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
  ],
  standalone: true
})
export class Cell implements OnInit {
  cellElement = viewChild<ElementRef<HTMLElement>>('cell');
  container = input<ElementRef<HTMLElement>>();
  parameters = input.required<CellModel>();
  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);
  private editMenuState = signal<EditMenuState | undefined>(undefined);
  currentCell = signal<CellModel | undefined>(undefined);
  isHighlighted = signal<boolean>(true);
  factoryId!: number;
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

  constructor(
    private cellStoreService: CellStoreService,
    private orderStoreService: OrderStoreService,
    private editMenuService: EditMenuService,
    private cellSettingsService: CellSettingsService,
    private factorySelectionService: FactorySelectionService
  ) {
    effect(() => {
      const parameters = this.parameters();
      untracked(() => {
        this.updateCell(parameters);
      });
    });
  }

  ngOnInit(): void {

    //react to the selection of a factory
    this.factorySelectionService.factorySelected$.subscribe({
      next: factory => {
        this.factoryId = factory?.id ?? 0;
      }
    });

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

    // React to updates to the cell settings
    this.cellSettingsService.cellSettingsChanged$.subscribe({
      next: newSettings => {
        if (!newSettings || newSettings?.cellId != this.currentCell()?.id) return;

        //set the new settings
        this.currentCell.set(<CellModel>{
          iconName: newSettings.cellSettings.icon ?? this.currentCell()?.iconName!,
          image: newSettings.cellSettings.image ?? ''
        });
      }
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

