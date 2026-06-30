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
import { LauncherLayout, LauncherLayoutService } from '../services/launcher-layout.service';
import { AboutDialog } from '../about-dialog/about-dialog';
import { ModuleItem } from '../models/module-item';
import { ModuleService } from '../services/module.service';
import { LocationPersistenceService } from '../services/location-persistence.service';
import { CultureService } from '../services/culture.service';
import { SearchService } from '../services/search.service';
import { ThemeMode, ThemeService } from '../services/theme.service';
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
  private locationPersistenceService = inject(LocationPersistenceService);
  private cultureService = inject(CultureService);
  private launcherLayoutService = inject(LauncherLayoutService);
  private searchService = inject(SearchService);
  private themeService = inject(ThemeService);
  private dialog = inject(MatDialog);

  protected TranslationConstants = TranslationConstants;
  currentTheme = this.themeService.mode;

  currentLayout = this.launcherLayoutService.layout;
  topRegionAvailable = this.launcherLayoutService.topRegionAvailable;
  topRegionEnabled = this.launcherLayoutService.topRegionEnabled;
  rightRegionAvailable = this.launcherLayoutService.rightRegionAvailable;
  rightRegionEnabled = this.launcherLayoutService.rightRegionEnabled;

  modules = this.moduleService.otherModules;
  supportedCultures = this.cultureService.supportedCultures;
  currentCulture = this.cultureService.currentCulture;

  @ViewChild('appMenu') appMenu!: MatMenu;
  protected readonly LauncherLayout = LauncherLayout;

  setLayout(layout: LauncherLayout) {
    this.launcherLayoutService.updateLayout(layout);
  }

  openAbout() {
    this.dialog.open(AboutDialog);
  }

  toggleTopRegion(): void {
    this.launcherLayoutService.updateTopRegionEnabled(!this.topRegionEnabled());
  }

  toggleRightRegion(): void {
    this.launcherLayoutService.updateRightRegionEnabled(!this.rightRegionEnabled());
  }

  openSpotlight(): void {
    this.searchService.open();
  }

  setTheme(mode: ThemeMode): void {
    this.themeService.setMode(mode);
  }

  resolveHref(module: ModuleItem): string {
    return this.locationPersistenceService.resolveHref(module);
  }

  selectCulture = this.cultureService.selectCulture.bind(this.cultureService);
}
