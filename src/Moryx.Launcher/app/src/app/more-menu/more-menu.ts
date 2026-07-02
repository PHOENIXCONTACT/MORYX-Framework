/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, viewChild } from '@angular/core';
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
  protected currentTheme = this.themeService.mode;

  protected currentLayout = this.launcherLayoutService.layout;
  protected topRegionAvailable = this.launcherLayoutService.topRegionAvailable;
  protected topRegionEnabled = this.launcherLayoutService.topRegionEnabled;
  protected rightRegionAvailable = this.launcherLayoutService.rightRegionAvailable;
  protected rightRegionEnabled = this.launcherLayoutService.rightRegionEnabled;

  protected otherModules = this.moduleService.otherModules;
  protected supportedCultures = this.cultureService.supportedCultures;
  protected currentCulture = this.cultureService.currentCulture;

  readonly appMenu = viewChild.required<MatMenu>('appMenu');
  protected readonly LauncherLayout = LauncherLayout;

  protected setLayout(layout: LauncherLayout) {
    this.launcherLayoutService.updateLayout(layout);
  }

  protected openAbout() {
    this.dialog.open(AboutDialog, { width: 'min(560px, 92vw)', minWidth: '280px' });
  }

  protected toggleTopRegion(): void {
    this.launcherLayoutService.updateTopRegionEnabled(!this.topRegionEnabled());
  }

  protected toggleRightRegion(): void {
    this.launcherLayoutService.updateRightRegionEnabled(!this.rightRegionEnabled());
  }

  protected openSpotlight(): void {
    this.searchService.open();
  }

  protected setTheme(mode: ThemeMode): void {
    this.themeService.setMode(mode);
  }

  protected resolveHref(module: ModuleItem): string {
    return this.locationPersistenceService.resolveHref(module);
  }

  protected selectCulture = this.cultureService.selectCulture.bind(this.cultureService);
}
