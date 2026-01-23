/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import Cell from 'src/app/models/cell';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
import { CellComponent } from '../cell/cell.component';
import { FactoryComponent } from '../factory/factory.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-factory-board',
  templateUrl: './factory-board.component.html',
  imports: [
    CellComponent,
    FactoryComponent,
    CommonModule
  ],
  styleUrl: './factory-board.component.scss'
})
export class FactoryBoardComponent implements OnInit {

  factoryId !: number | undefined;
  constructor(    
    public elemRef: ElementRef,
    public factorySelectionService: FactorySelectionService,
    public cellStoreService: CellStoreService,
    private route: ActivatedRoute,){

    }

  ngOnInit(): void {

    this.factoryId = Number(this.route.snapshot.paramMap.get("id"));

    //use the default factory when no factory id provided in the url
    this.factorySelectionService.defaultFactory$.subscribe(item => {
      if(this.factoryId != undefined && this.factoryId > 0) return;
      if(!item) return;

      this.factorySelectionService.selectFactory(item.id);
    });


    if(this.factoryId != undefined && this.factoryId > 0)     
    //select a new factory based on the id in the url
    this.factorySelectionService.selectFactory(this.factoryId);



  }

  getCell(cellId: number): Cell{
    const output = this.cellStoreService.getCell(cellId) ?? <Cell> {};
    return output;
  }

}

