/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ChangeBackgroundDialog } from '@app/dialogs/change-background-dialog/change-background-dialog';
import { CellStoreService } from '@app/services/cell-store.service';
import { EditMenuService } from '@app/services/edit-menu.service';
import { EditMenuState } from '@app/services/EditMenutState';
import { TranslationConstants } from '@app/translation-constants';
import { TranslatePipe } from '@ngx-translate/core';
import { FactorySelectionService } from '@app/services/factory-selection.service';
import { FactoryMonitorService } from '@api/services';
import { ChangeBackgroundService } from '@app/services/change-background.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-edit-menu',
  templateUrl: './edit-menu.html',
  styleUrls: ['./edit-menu.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTooltipModule,
    MatIconModule,
    MatButtonModule,
    TranslatePipe
  ]
})
export class EditMenu {
  private editMenuService = inject(EditMenuService);
  private matDialog = inject(MatDialog);
  private router = inject(Router);
  private factorySelectionService = inject(FactorySelectionService);
  private factoryMonitorService = inject(FactoryMonitorService);
  private cellStoreService = inject(CellStoreService);
  private backgroundService = inject(ChangeBackgroundService);

  private menuButtons = [
    {
      icon: 'open_with',
      state: EditMenuState.EditingCells,
    },
    {
      icon: 'wallpaper',
      state: EditMenuState.EditingBackground,
    },
  ];
  protected editingEnabled = signal(false);
  protected buttons = signal<{ icon: string; state: EditMenuState }[]>([]);
  protected activeState = signal<EditMenuState>(EditMenuState.Closed);
  protected backgroundState: EditMenuState = EditMenuState.EditingBackground;
  protected TranslationConstants = TranslationConstants;
  protected canGoBack = signal(false);
  private goBackToFactory!: number;

  constructor() {
    effect(() => {
      const factory = this.factorySelectionService.factorySelected();
      if (!factory) {
        this.canGoBack.set(false);
        return;
      }

      this.factoryMonitorService.getNavigation({factoryId: factory}).then(navigation => {
        this.backgroundService.updateBackground(navigation.backgroundURL);

        if (!navigation.parentId) {
          this.canGoBack.set(false);
          return;
        }

        this.canGoBack.set(true);
        this.goBackToFactory = navigation.parentId ?? 0;
      });
    });
  }

  protected onToggleEditingMode() {
    this.editingEnabled.update(value => !value);
    if (this.editingEnabled()) {
      this.showMenu();
    } else {
      this.hideMenu();
    }
  }

  protected goBack() {
    this.router.navigate(['/factory', this.goBackToFactory]).then(() => {
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.goBackToFactory);
    });
  }

  private showMenu() {
    this.buttons.set(this.menuButtons);
  }

  private hideMenu() {
    this.buttons.set([]);
    this.editMenuService.setActiveState(EditMenuState.Closed);
  }

  protected onClickMenuButton(state: EditMenuState) {
    switch (state) {
      case EditMenuState.EditingCells:
        this.editMenuService.setActiveState(EditMenuState.EditingCells);
        break;
      case EditMenuState.EditingRoutes:
        this.editMenuService.setActiveState(EditMenuState.EditingRoutes);
        break;
      case EditMenuState.EditingBackground:
        this.editMenuService.setActiveState(EditMenuState.EditingBackground);
        //display the change background dialog
        this.matDialog.open(ChangeBackgroundDialog, {});
        break;
      default:
        break;
    }
  }
}

