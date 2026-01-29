/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, OnInit } from '@angular/core';
import { EditMenuService } from './services/edit-menu.service';
import { EditMenuState } from './services/EditMenutState';
import { ChangeBackgroundService } from './services/change-background.service';
import { LanguageService } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { FactorySelectionService } from './services/factory-selection.service';
import { CellStoreService } from './services/cell-store.service';
import Cell from './models/cell';
import { EditMenuComponent } from './components/edit-menu/edit-menu.component';
import { OrdersContainerComponent } from './components/orders-container/orders-container.component';
import { CellDetailsComponent } from './components/cell-details/cell-details.component';

import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [
    EditMenuComponent,
    OrdersContainerComponent,
    CellDetailsComponent,
    RouterOutlet
],
    standalone: true
})
export class AppComponent implements OnInit {
  backgroundImage: string = '';
  private editMenuState !: EditMenuState;

  constructor(private editMenuService: EditMenuService, 
    private backgroundService: ChangeBackgroundService, 
    private languageService: LanguageService,
    public translate: TranslateService,
    public elemRef: ElementRef,
    public cellStoreService: CellStoreService,
    public factorySelectionService: FactorySelectionService) {
    this.editMenuService.activeState$.subscribe({
      next : state => this.editMenuState = state
    });

    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.backgroundService.backgroundChanged$.subscribe({
      next: url => this.backgroundImage = url 
    });

    this.factorySelectionService.factorySelected$.subscribe({
      next: factory => this.backgroundImage = factory?.backgroundURL ?? ''
    });
  }

  getCell(cellId: number): Cell{
    const output = this.cellStoreService.getCell(cellId) ?? <Cell> {};
    return output;
  }

  get isEditMode(){
    return this.editMenuState === EditMenuState.EditingCells;
  }
}

