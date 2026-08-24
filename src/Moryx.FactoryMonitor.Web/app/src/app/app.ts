/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { EditMenuService } from './services/edit-menu.service';
import { EditMenuState } from './services/EditMenutState';
import { ChangeBackgroundService } from './services/change-background.service';
import { EditMenu } from './components/edit-menu/edit-menu';
import { OrdersContainer } from './components/orders-container/orders-container';
import { CellDetails } from './components/cell-details/cell-details';
import { RouterOutlet } from '@angular/router';
import { FactoryStateStreamService } from './services/factory-state-stream.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    EditMenu,
    OrdersContainer,
    CellDetails,
    RouterOutlet
  ],
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class App implements OnInit {
  private editMenuService = inject(EditMenuService);
  private changeBackgroundService = inject(ChangeBackgroundService);
  private factoryStateStreamService = inject(FactoryStateStreamService);
  private destroyRef = inject(DestroyRef);

  protected backgroundImage = computed(() => {
    const bg = this.changeBackgroundService.backgroundChanged();
    return bg ? `url(${bg})` : 'none';
  });
  protected isEditMode = computed(() => this.editMenuService.activeState() === EditMenuState.EditingCells);

  constructor() {
    this.destroyRef.onDestroy(() => this.disconnectEvents());
  }

  ngOnInit(): void {
    this.factoryStateStreamService.connect();
  }

  protected disconnectEvents(): void {
    this.factoryStateStreamService.disconnect();
  }
}
