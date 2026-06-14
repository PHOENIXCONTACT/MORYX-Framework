import {Component, inject, ViewChild} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatMenu, MatMenuModule} from '@angular/material/menu';
import {MatDividerModule} from '@angular/material/divider';
import {MatDialog} from '@angular/material/dialog';
import {LauncherStateService} from '../services/launcher-state.service';
import {AboutDialog} from '../about-dialog/about-dialog';
import {ModuleService} from '../services/module.service';
import {CultureService} from '../services/culture.service';

@Component({
  selector: 'app-more-menu',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, MatDividerModule],
  templateUrl: './more-menu.html',
  styleUrl: './more-menu.scss'
})
export class MoreMenu {
  private moduleService = inject(ModuleService);
  private cultureService = inject(CultureService);

  modules = this.moduleService.otherModules;
  supportedCultures = this.cultureService.supportedCultures;
  currentCulture = this.cultureService.currentCulture;

  @ViewChild('appMenu') appMenu!: MatMenu;

  private launcherStateService = inject(LauncherStateService);
  currentState = this.launcherStateService.state;

  setLayout(mode: 'full' | 'operator' | 'fullscreen') {
    this.launcherStateService.updateState({
      fullscreen: mode === 'fullscreen',
      operatorMode: mode === 'operator',
    });
  }

  private dialog = inject(MatDialog);

  openAbout() {
    this.dialog.open(AboutDialog);
  }

  selectCulture = this.cultureService.selectCulture.bind(this.cultureService);
}
