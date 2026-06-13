import {Component, computed, input, signal} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {CultureModel, ExternalModuleItem, ModuleCategory, WebModuleItem} from '../web-module-item';
import {VerticalNav} from '../vertical-nav/vertical-nav';
import {MoryxLogo} from '../moryx-logo/moryx-logo';
import {MoreMenu} from '../more-menu/more-menu';

@Component({
  selector: 'app-full-layout',
  imports: [RouterOutlet, VerticalNav, MatToolbarModule, MatIconModule, MatButtonModule, MatSidenavModule, MoryxLogo, MoreMenu],
  templateUrl: './full-layout.html',
  styleUrl: './full-layout.scss'
})
export class FullLayout {

  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();

  userModules = computed(() => {
    return [...this.webModuleItems(), ...this.externalModuleItems()]
      .filter(m => m.category === ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex);
  });

  otherModules = computed(() => {
    return this.webModuleItems()
      .filter(m => m.category !== ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex);
  });

  navCollapsed = signal(false);

  toggleNav() {
    this.navCollapsed.update(v => !v);
  }
}
