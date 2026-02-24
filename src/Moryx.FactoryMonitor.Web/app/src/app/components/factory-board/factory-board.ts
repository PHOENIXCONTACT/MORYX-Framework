/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import CellModel from 'src/app/models/cellModel';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
import { Cell } from '../cell/cell';
import { Factory } from '../factory/factory';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-factory-board',
  templateUrl: './factory-board.html',
  imports: [
    Cell,
    Factory,
    CommonModule
  ],
  styleUrl: './factory-board.scss'
})
export class FactoryBoard implements OnInit {
  elemRef = inject(ElementRef);
  private factorySelectionService = inject(FactorySelectionService);
  private cellStoreService = inject(CellStoreService);
  private activatedRoute = inject(ActivatedRoute);
  factoryContent = toSignal(this.factorySelectionService.factoryContent$);
  factoryId !: number | undefined;

  ngOnInit(): void {

    this.factoryId = Number(this.activatedRoute.snapshot.paramMap.get('id'));

    // ToDo: To set the default factory we currently meander from the api call in the
    // cell-store.service to the factory-selection.service into the factory-board back into the service.
    // This can be drastically simplified by setting the default factory-content automatically. Also route
    // changes coulde be processed in the serive making this a pure display component.
    //use the default factory when no factory id provided in the url
    this.factorySelectionService.defaultFactory$.subscribe(item => {
      if (this.factoryId != undefined && this.factoryId > 0) return;
      if (!item) return;

      this.factorySelectionService.selectFactory(item.id);
    });


    if (this.factoryId != undefined && this.factoryId > 0)
      //select a new factory based on the id in the url
      this.factorySelectionService.selectFactory(this.factoryId);
  }

  getCell(cellId: number): CellModel {
    const output = this.cellStoreService.getCell(cellId) ?? <CellModel>{};
    return output;
  }
}
