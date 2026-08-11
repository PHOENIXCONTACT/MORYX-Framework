/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { VerticalModuleNav } from '../../navigation/vertical-module-nav/vertical-module-nav';
import { MoreMenu } from '../../more-menu/more-menu';
import { MatMenuTrigger } from '@angular/material/menu';
import { AuthButton } from '../../auth-button/auth-button';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';
import { ToolbarSearch } from '../../toolbar-search/toolbar-search';
import { LayoutBase } from '../layout-base';

@Component({
  selector: 'app-full-layout',
  imports: [
    VerticalModuleNav,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatSidenavModule,
    MoreMenu,
    MatMenuTrigger,
    AuthButton,
    ToolbarSearch
  ],
  templateUrl: './full-layout.html',
  styleUrl: './full-layout.scss'
})
export class FullLayout extends LayoutBase {
  private authService = inject(AuthService);
  protected environment = environment;

  protected navCollapsed = this.launcherLayoutService.navCollapsed;
  protected authConfigured = this.authService.authConfigured;

  protected toggleNav() {
    this.launcherLayoutService.updateNavCollapsed(!this.navCollapsed());
  }
}
