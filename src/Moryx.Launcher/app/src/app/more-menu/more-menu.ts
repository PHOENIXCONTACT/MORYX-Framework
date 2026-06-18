/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenu, MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { LauncherLayout, LauncherStateService } from '../services/launcher-state.service';
import { AboutDialog } from '../about-dialog/about-dialog';
import { ModuleService } from '../services/module.service';
import { CultureService } from '../services/culture.service';
import { TranslationConstants } from '../translation-constants';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-more-menu',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, MatDividerModule, TranslatePipe],
  templateUrl: './more-menu.html',
  styleUrl: './more-menu.scss'
})
export class MoreMenu {
  private moduleService = inject(ModuleService);
  private cultureService = inject(CultureService);

  TranslationConstants = TranslationConstants;

  modules = this.moduleService.otherModules;
  supportedCultures = this.cultureService.supportedCultures;
  currentCulture = this.cultureService.currentCulture;

  @ViewChild('appMenu') appMenu!: MatMenu;

  private launcherStateService = inject(LauncherStateService);
  currentLayout = this.launcherStateService.layout;
  protected readonly LauncherLayout = LauncherLayout;

  setLayout(layout: LauncherLayout) {
    this.launcherStateService.updateLayout(layout);
  }

  private dialog = inject(MatDialog);

  openAbout() {
    this.dialog.open(AboutDialog);
  }

  selectCulture = this.cultureService.selectCulture.bind(this.cultureService);
}
