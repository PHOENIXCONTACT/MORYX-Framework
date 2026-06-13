import {Component, computed, effect, input, signal, ViewEncapsulation} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {ExternalModuleItem, ModuleCategory, WebModuleItem} from './web-module-item';
import {VerticalNav} from './vertical-nav/vertical-nav';
import {MoryxLogo} from './moryx-logo/moryx-logo';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, VerticalNav, MatToolbarModule, MatIconModule, MatButtonModule, MatSidenavModule, MoryxLogo],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.ShadowDom
})
export class App {

  // Web components inputs
  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();

  userModules = computed(() => {
    return [...this.webModuleItems(), ...this.externalModuleItems()]
      .filter(m => m.category === ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex);
  });

  navCollapsed = signal(false);

  constructor() {
    effect(() => {
      console.log('Module items changed', this.userModules());
    });
  }

  toggleNav() {
    this.navCollapsed.update(v => !v);
  }
}
