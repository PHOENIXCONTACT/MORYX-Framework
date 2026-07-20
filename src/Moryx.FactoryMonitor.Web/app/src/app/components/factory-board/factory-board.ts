/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, ElementRef, inject, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FactorySelectionService } from '@app/services/factory-selection.service';
import { Cell } from '../cell/cell';
import { Factory } from '../factory/factory';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-factory-board',
  templateUrl: './factory-board.html',
  imports: [
    Cell,
    Factory,
    CommonModule
  ],
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './factory-board.scss'
})
export class FactoryBoard {
  protected elemRef = inject(ElementRef);
  private factorySelectionService = inject(FactorySelectionService);
  private activatedRoute = inject(ActivatedRoute);
  protected factoryContent = this.factorySelectionService.factoryContent;
  private factoryId = Number(this.activatedRoute.snapshot.paramMap.get('id'));

  constructor() {
    // ToDo: To set the default factory we currently meander from the api call in the
    // cell-store.service to the factory-selection.service into the factory-board back into the service.
    // This can be drastically simplified by setting the default factory-content automatically. Also route
    // changes coulde be processed in the serive making this a pure display component.
    //use the default factory when no factory id provided in the url
    // Solution: Use Route Resolver to prepare data before component actually is loaded
    effect(() => {
      const item = this.factorySelectionService.defaultFactory();
      if (this.factoryId != undefined && this.factoryId > 0) {
        return;
      }
      if (!item) {
        return;
      }

      this.factorySelectionService.selectFactory(item.id);
    });

    if (this.factoryId != undefined && this.factoryId > 0) {
      //select a new factory based on the id in the url
      this.factorySelectionService.selectFactory(this.factoryId);
    }
  }
}
