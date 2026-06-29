/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnInit, ChangeDetectionStrategy, DestroyRef } from '@angular/core';
import { EditMenuService } from './services/edit-menu.service';
import { EditMenuState } from './services/EditMenutState';
import { ChangeBackgroundService } from './services/change-background.service';
import { LanguageService } from '@moryx/ngx-web-framework/services';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { CellStoreService } from './services/cell-store.service';
import CellModel from './models/cellModel';
import { EditMenu } from './components/edit-menu/edit-menu';
import { OrdersContainer } from './components/orders-container/orders-container';
import { CellDetails } from './components/cell-details/cell-details';
import { RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
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
  private factoryStateStreamService = inject(FactoryStateStreamService);
  private languageService = inject(LanguageService);
  private translateService = inject(TranslateService);
  private cellStoreService = inject(CellStoreService);
  private destroyRef = inject(DestroyRef);

  private editMenuState = toSignal(inject(EditMenuService).activeState$, { initialValue: EditMenuState.Closed });
  private background = toSignal(inject(ChangeBackgroundService).backgroundChanged$);
  backgroundImage = computed(() => {
    const bg = this.background();
    return bg ? `url(${bg})` : 'none';
  });
  isEditMode = computed(() => this.editMenuState() === EditMenuState.EditingCells);

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang('en');
    this.translateService.use(this.languageService.getFallbackLang());
    this.destroyRef.onDestroy(() => this.disconnectEvents());
  }

  ngOnInit(): void {
    this.factoryStateStreamService.connect();
  }

  getCell(cellId: number): CellModel {
    const output = this.cellStoreService.getCell(cellId) ?? <CellModel>{};
    return output;
  }

  disconnectEvents(): void {
    this.factoryStateStreamService.disconnect();
  }
}
