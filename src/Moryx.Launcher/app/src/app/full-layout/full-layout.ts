import {Component, inject} from '@angular/core';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {VerticalModuleNav} from '../vertical-module-nav/vertical-module-nav';
import {MoryxLogo} from '../moryx-logo/moryx-logo';
import {MoreMenu} from '../more-menu/more-menu';
import {MatMenuTrigger} from '@angular/material/menu';
import {LauncherStateService} from '../services/launcher-state.service';
import {AuthButton} from '../auth-button/auth-button';
import {AuthService} from '../services/auth.service';

@Component({
  selector: 'app-full-layout',
  imports: [VerticalModuleNav, MatToolbarModule, MatIconModule, MatButtonModule, MatSidenavModule, MoryxLogo, MoreMenu, MatMenuTrigger, AuthButton],
  templateUrl: './full-layout.html',
  styleUrl: './full-layout.scss'
})
export class FullLayout {
  private authService = inject(AuthService);
  private launcherStateService = inject(LauncherStateService);

  navCollapsed = this.launcherStateService.navCollapsed;
  authConfigured = this.authService.authConfigured;

  toggleNav() {
    this.launcherStateService.updateNavCollapsed(!this.navCollapsed());
  }
}
