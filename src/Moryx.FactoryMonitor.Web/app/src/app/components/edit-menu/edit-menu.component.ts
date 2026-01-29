/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ChangeBackgroundDialogComponent } from 'src/app/dialogs/change-background-dialog/change-background-dialog.component';
import { CellStoreService } from 'src/app/services/cell-store.service';
import { EditMenuService } from 'src/app/services/edit-menu.service';
import { EditMenuState } from 'src/app/services/EditMenutState';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { TranslateService, TranslatePipe, TranslateModule } from '@ngx-translate/core';
import { FactorySelectionService } from 'src/app/services/factory-selection.service';
import { FactoryMonitorService } from 'src/app/api/services';
import { ChangeBackgroundService } from 'src/app/services/change-background.service';
import { FactoryModel } from 'src/app/api/models/factory-model';
import { MatButtonModule, MatFabButton, MatMiniFabButton } from '@angular/material/button';
import { MatTooltip, MatTooltipModule } from '@angular/material/tooltip';
import { MatIcon, MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-edit-menu',
  templateUrl: './edit-menu.component.html',
  styleUrls: ['./edit-menu.component.scss'],
  imports: [
    MatTooltipModule,
    MatIconModule,
    MatButtonModule,
    TranslateModule
],
  standalone: true,
})
export class EditMenuComponent implements OnInit {
  private menuButtons = [
    {
      icon: 'open_with',
      state: EditMenuState.EditingCells,
    },
    /*{
      icon: 'route',
      state: EditMenuState.EditingRoutes,
    },*/
    {
      icon: 'wallpaper',
      state: EditMenuState.EditingBackground,
    },
  ];
  editingEnabled = signal(false);
  buttons: any[] = [];
  activeState: EditMenuState = EditMenuState.Closed;
  backgroundState: EditMenuState = EditMenuState.EditingBackground;
  TranslationConstants = TranslationConstants;
  canGoBack: boolean = false;
  goBackToFactory!: number;
  navigationItem!: FactoryModel;

  constructor(
    public editMenuService: EditMenuService,
    public matDialog: MatDialog,
    public translate: TranslateService,
    private router: Router,
    private factorySelectionService: FactorySelectionService,
    private factoryMonitorService: FactoryMonitorService,
    private cellStoreService: CellStoreService,
    private backgroundService: ChangeBackgroundService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
  }

  ngOnInit(): void {
    this.factorySelectionService.factorySelected$.subscribe({
      next: factory => {
        if (!factory) {
          this.canGoBack = false;
          return;
        }

        this.factoryMonitorService.getNavigation({ factoryId: factory.id ?? 0 }).subscribe(navigation => {
          this.navigationItem = navigation;
          this.backgroundService.changeLocalBackground(navigation.backgroundURL ?? '');
          
          if (!navigation.parentId) {
            this.canGoBack = false;
            return;
          }

          this.canGoBack = true;
          this.goBackToFactory = navigation.parentId ?? 0;
        });
      },
    });
  }

  onToggleEditingMode() {
    this.editingEnabled.update(value => !value);
    if (this.editingEnabled()) {
      this.showMenu();
    } else {
      this.hideMenu();
    }
  }

  goBack() {
    this.router.navigate(['/factory', this.goBackToFactory]).then(() => {
      this.cellStoreService.selectCell(undefined);
      this.factorySelectionService.selectFactory(this.goBackToFactory);
    });
  }

  showMenu() {
    this.buttons = this.menuButtons;
  }

  hideMenu() {
    this.buttons = [];
    this.editMenuService.setActiveState(EditMenuState.Closed);
  }

  onClickMenuButton(state: EditMenuState) {
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
        this.matDialog.open(ChangeBackgroundDialogComponent, {});
        break;
      default:
        break;
    }
  }
}

